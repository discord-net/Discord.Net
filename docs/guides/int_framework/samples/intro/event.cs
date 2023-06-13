// Theres multiple ways to subscribe to the event, depending on your application. Please use the approach fit to your type of client.
// DiscordSocketClient:
_socketClient.InteractionCreated += async (x) =>
{
    var ctx = new SocketInteractionContext(_socketClient, x);
    await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
}

// DiscordShardedClient:
_shardedClient.InteractionCreated += async (x) =>
{
    var ctx = new ShardedInteractionContext(_shardedClient, x);
    await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
}
