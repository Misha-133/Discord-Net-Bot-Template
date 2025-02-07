using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using DiscordNetTemplate.Options;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordNetTemplate.Services;

public class DiscordBotService(DiscordSocketClient client, InteractionService interactions, ILogger<DiscordBotService> logger, IOptions<DiscordBotOptions> options) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        client.Ready += ClientReady;

        client.Log += LogAsync;
        interactions.Log += LogAsync;

        return client.LoginAsync(TokenType.Bot, options.Value.Token)
            .ContinueWith(t => client.StartAsync(), cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        if (ExecuteTask is null)
            return Task.CompletedTask;

        base.StopAsync(cancellationToken);
        return client.StopAsync();
    }

    private async Task ClientReady()
    {
        logger.LogInformation("Logged as {User}", client.CurrentUser);

        var commands = await interactions.RegisterCommandsGloballyAsync();

        logger.LogInformation("Registered {Count} commands", commands.Count);
    }

    public Task LogAsync(LogMessage msg)
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

        logger.Log(severity, msg.Exception, msg.Message);

        return Task.CompletedTask;
    }
}