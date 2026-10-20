using System.Net.Http;
using JobyAI.Api.AI;
using JobyAI.Api.Configuration.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OllamaSharp;
using OllamaSharp.Models;

namespace JobyAI.Api.AI;

public sealed class OllamaProvider : IAiProvider, IDisposable
{
    private readonly OllamaApiClient _ollamaClient;
    private readonly IChatClient _chatClient;
    private readonly AiOptions _options;
    private readonly ILogger<OllamaProvider> _logger;

    public OllamaProvider(IOptions<AiOptions> options, ILogger<OllamaProvider> logger)
    {
        _options = options.Value;
        _logger = logger;

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_options.BaseUrl),
            Timeout = Timeout.InfiniteTimeSpan
        };

        _ollamaClient = new OllamaApiClient(httpClient, _options.Model);

        _chatClient = _ollamaClient;
    }

    public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Check Ollama and the configured model first.
        await EnsureReadyAsync(cancellationToken);

        // Apply a separate timeout to model generation.
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(_options.RequestTimeoutSeconds));

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,timeoutCts.Token);

        try
        {
            return await _chatClient.GetResponseAsync(messages, options, linkedCts.Token);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            // The caller cancelled the operation.
            throw;
        }
        catch (OperationCanceledException ex) when (timeoutCts.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Ollama request timed out after {TimeoutSeconds} seconds.", _options.RequestTimeoutSeconds);

            throw new AiProviderException(AiProviderFailure.Timeout, $"Ollama did not respond within " + $"{_options.RequestTimeoutSeconds} seconds.", ex);
        }
        catch (HttpRequestException ex) when (ex.StatusCode is null)
        {
            _logger.LogWarning(ex, "Could not connect to Ollama at {BaseUrl}.", _options.BaseUrl);

            throw new AiProviderException(AiProviderFailure.OllamaUnavailable, "Ollama became unavailable. Check that it is running.", ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Ollama failed while generating a response.");

            throw new AiProviderException(AiProviderFailure.RequestFailed, "Ollama failed to generate a response. " + "Check the application logs for details.", ex);
        }
    }

    private async Task EnsureReadyAsync(CancellationToken cancellationToken)
    {
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(_options.HealthCheckTimeoutSeconds));

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        IEnumerable<Model> models;

        try
        {
            models = await _ollamaClient.ListLocalModelsAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException ex)
        {
            throw new AiProviderException(AiProviderFailure.OllamaUnavailable, "The Ollama health check timed out. " + "Check that Ollama is running.", ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Could not check Ollama at {BaseUrl}.", _options.BaseUrl);

            throw new AiProviderException(AiProviderFailure.OllamaUnavailable, $"Could not connect to Ollama at {_options.BaseUrl}. " + "Start Ollama and try again.", ex);
        }

        var configuredModel = NormalizeModelName(_options.Model);

        var modelExists = models.Any(model => string.Equals( NormalizeModelName(model.Name), configuredModel, StringComparison.OrdinalIgnoreCase));

        if (!modelExists)
        {
            throw new AiProviderException(AiProviderFailure.ModelMissing, $"The model '{_options.Model}' is not installed. " + $"Install it by running: ollama pull {_options.Model}");
        }
    }

    private static string NormalizeModelName(string name)
    {
        name = name.Trim();

        return name.Contains(':')
            ? name
            : $"{name}:latest";
    }

    public void Dispose()
    {
        _ollamaClient.Dispose();
    }
}