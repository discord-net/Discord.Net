discordClient.ButtonExecuted += async (interaction) => 
{
    var ctx = new SocketInteractionContext<SocketMessageComponent>(discordClient, interaction);
    await _interactionService.ExecuteAsync(ctx, serviceProvider);
};

public class MessageComponentModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("custom_id")]
    public async Command()
    {
        Context.Interaction.UpdateAsync(...);
    }
}
