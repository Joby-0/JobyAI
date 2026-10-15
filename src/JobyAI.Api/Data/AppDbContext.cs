using Microsoft.EntityFrameworkCore;

namespace JobyAI.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}
