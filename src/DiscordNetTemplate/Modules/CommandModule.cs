namespace DiscordNetTemplate.Modules;

public class CommandModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<CommandModule> _logger;

    public CommandModule(ILogger<CommandModule> logger)
    {
        _logger = logger;
    }

    [SlashCommand("test", "Just a test command")]
    public async Task TestCommand()
        => await RespondAsync("Hello There");

}