
// This parameteres can be configured on the module level
// Set supported command context types to Bot DMs and Private Channels (regular DM & GDM)
[CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel)]
// Set supported integration installation type to User Install
[IntegrationType(ApplicationIntegrationType.UserInstall)]
public class CommandModule() : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("test", "Just a test command")]
	public async Task TestCommand()
		=> await RespondAsync("Hello There");

    // But can also be overridden on the command level
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
    [IntegrationType(ApplicationIntegrationType.GuildInstall)]
    [SlashCommand("echo", "Echo the input")]
	public async Task EchoCommand(string input)
		=> await RespondAsync($"You said: {input}");
}
