using JobyAI.Api.AI;
using JobyAI.Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Api.Configuration.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("JobyAI")
    ?? throw new InvalidOperationException("Connection string 'JobyAI' is not configured.");
var sqliteConnection = new SqliteConnectionStringBuilder(connectionString);

if (!string.Equals(sqliteConnection.DataSource, ":memory:", StringComparison.OrdinalIgnoreCase)
    && !Path.IsPathRooted(sqliteConnection.DataSource))
{
    sqliteConnection.DataSource = Path.GetFullPath(
        sqliteConnection.DataSource,
        builder.Environment.ContentRootPath);
}

var databaseDirectory = Path.GetDirectoryName(sqliteConnection.DataSource);
if (!string.IsNullOrEmpty(databaseDirectory))
{
    Directory.CreateDirectory(databaseDirectory);
}

builder.Services.AddDbContext<MainDbContext>(options => options.UseSqlite(sqliteConnection.ToString()));

builder.Services.AddOptions<AiOptions>().BindConfiguration("AI")
    .Validate(
        options => Uri.TryCreate(
            options.BaseUrl,
            UriKind.Absolute,
            out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp ||
             uri.Scheme == Uri.UriSchemeHttps),
        "AI:BaseUrl must be a valid HTTP or HTTPS URL.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.Model),
        "AI:Model is required.")
    .ValidateOnStart();

builder.Services.AddSingleton<IAiProvider, OllamaProvider>();

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/scalar", options =>
        options.WithTitle("JobyAI API"));
}

app.Run();