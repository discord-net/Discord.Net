discordClient.ButtonExecuted += async (interaction) => 
{
    var ctx = new SocketInteractionContext<SocketMessageComponent>(discordClient, interaction);
    await _interactionService.ExecuteCommandAsync(ctx, serviceProvider);
};

public class MessageComponentModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("custom_id")]
    public async Task Command()
    {
        await Context.Interaction.UpdateAsync(...);
    }
}
