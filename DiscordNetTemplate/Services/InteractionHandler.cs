using System.Reflection;

namespace DiscordNetTemplate.Services;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _services;
    private readonly ILogger _logger;

    public InteractionHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider services, ILogger<InteractionHandler> logger)
    {
        _client = client;
        _interactionService = interactionService;
        _services = services;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.InteractionCreated += HandleInteraction;
        _interactionService.InteractionExecuted += HandleInteractionExecuted;
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(_client, interaction);

            var result = await _interactionService.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess)
                _ = Task.Run(() => HandleInteractionExecutionResult(interaction, result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    private Task HandleInteractionExecuted(ICommandInfo command, IInteractionContext context, IResult result)
    {
        if (!result.IsSuccess)
            _ = Task.Run(() => HandleInteractionExecutionResult(context.Interaction, result));
        return Task.CompletedTask;
    }

    private async Task HandleInteractionExecutionResult(IDiscordInteraction interaction, IResult result)
    {
        switch (result.Error)
        {
            case InteractionCommandError.UnmetPrecondition:
                _logger.LogInformation($"Unmet precondition - {result.Error}");
                break;

            case InteractionCommandError.BadArgs:
                _logger.LogInformation($"Unmet precondition - {result.Error}");
                break;

            case InteractionCommandError.ConvertFailed:
                _logger.LogInformation($"Convert Failed - {result.Error}");
                break;

            case InteractionCommandError.Exception:
                _logger.LogInformation($"Exception - {result.Error}");
                break;

            case InteractionCommandError.ParseFailed:
                _logger.LogInformation($"Parse Failed - {result.Error}");
                break;

            case InteractionCommandError.UnknownCommand:
                _logger.LogInformation($"Unknown Command - {result.Error}");
                break;

            case InteractionCommandError.Unsuccessful:
                _logger.LogInformation($"Unsuccessful - {result.Error}");
                break;
        }

        if (!interaction.HasResponded)
        {
            await interaction.RespondAsync("An error has occurred. We are already investigating it!", ephemeral: true);
        }
        else
        {
            await interaction.FollowupAsync("An error has occurred. We are already investigating it!", ephemeral: true);
        }
    }
}