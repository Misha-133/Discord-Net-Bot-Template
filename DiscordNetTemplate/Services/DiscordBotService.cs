using Microsoft.Extensions.Hosting;

namespace DiscordNetTemplate.Services;

public class DiscordBotService : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactions;
    private readonly IConfiguration _config;
    private readonly ILogger _logger;
    private readonly InteractionHandler _interactionHandler;

    public DiscordBotService(DiscordSocketClient client, InteractionService interactions, IConfiguration config, ILogger<DiscordBotService> logger, InteractionHandler interactionHandler)
    {
        _client = client;
        _interactions = interactions;
        _config = config;
        _logger = logger;
        _interactionHandler = interactionHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Ready += ClientReady;

        _client.Log += LogAsync;
        _interactions.Log += LogAsync;

        await _interactionHandler.InitializeAsync();

        await _client.LoginAsync(TokenType.Bot, _config["Secrets:Discord"]);

        await _client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.StopAsync();
    }

    private async Task ClientReady()
    {
        _logger.LogInformation($"Logged as {_client.CurrentUser}");

        await _interactions.RegisterCommandsGloballyAsync();
    }

    public async Task LogAsync(LogMessage msg)
    {
        var severity = msg.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information
        };

        _logger.Log(severity, msg.Exception, msg.Message);

        await Task.CompletedTask;
    }
}