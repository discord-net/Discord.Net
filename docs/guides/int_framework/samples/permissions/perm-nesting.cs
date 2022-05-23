[EnabledInDm(true)]
[DefaultMemberPermissions(GuildPermission.ViewChannels)]
public class Module : InteractionModuleBase<SocketInteractionContext>
{
    [DefaultMemberPermissions(GuildPermission.SendMessages)]
    public class NestedModule : InteractionModuleBase<SocketInteractionContext>
    {
        // While looking for more permissions, it has found 'ViewChannels' and 'SendMessages'. The result of this lookup will be:
        // ViewChannels + SendMessages + ManageMessages.
        // If these together are not found for target user, the command will not show up for them.
        [DefaultMemberPermissions(GuildPermission.ManageMessages)]
        [SlashCommand("ping", "Pong!")]
        public async Task Ping()
            => await RespondAsync("pong");
    }
}
