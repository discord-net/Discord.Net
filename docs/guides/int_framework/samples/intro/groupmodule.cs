// You can put commands in groups
[Group("group-name", "Group description")]
public class CommandGroupModule : InteractionModuleBase<SocketInteractionContext>
{
    // This command will look like
    // group-name ping
    [SlashCommand("ping", "Get a pong")]
    public async Task PongSubcommand()
        => await RespondAsync("Pong!");
    
    // And even in sub-command groups
    [Group("subcommand-group-name", "Subcommand group description")]
    public class SubСommandGroupModule : InteractionModuleBase<SocketInteractionContext>
    {
        // This command will look like
        // group-name subcommand-group-name echo
        [SlashCommand("echo", "Echo an input")]
        public async Task EchoSubcommand(string input)
            => await RespondAsync(input);
    }
}