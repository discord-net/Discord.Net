[Group("group-name", "Group description")]
public class CommandGroupModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Get a pong")]
    public async Task PongSubcommand()
        => await RespondAsync("Pong!");

    [Group("subcommand-group-name", "Subcommand group description")]
    public class SubcommandGroupModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("echo", "Echo an input")]
        public async Task PongSubcommand(string input)
            => await RespondAsync(input);
    }
}