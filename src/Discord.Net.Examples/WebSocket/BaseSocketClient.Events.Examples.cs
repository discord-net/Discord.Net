#region ReactionAdded
public void HookReactionAdded(BaseSocketClient client)
{
    client.ReactionAdded += HandleReactionAddedAsync;
}
public async Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> cachedMessage, 
    ISocketMessageChannel originChannel, SocketReaction reaction)
{
    var message = await cachedMessage.GetOrDownloadAsync();
    if (message != null && reaction.User.IsSpecified)
        Console.WriteLine($"{message.User.Value} just added a reaction '{reaction.Emote}' " + 
            $"to {message.Author}'s message ({message.Id}).");
}
#endregion

#region ChannelCreated
public void HookChannelCreated(BaseSocketClient client)
{
    client.ChannelCreated += HandleChannelCreated;
}
public Task HandleChannelCreated(SocketChannel channel)
{
    Console.WriteLine($"A new channel '{channel.Name}'({channel.Id}, {channel.GetType()})"
        + $"has been created at {channel.CreatedAt}.");
    return Task.CompletedTask;
}
#endregion

#region ChannelDestroyed
public void HookChannelDestroyed(BaseSocketClient client)
{
    client.ChannelDestroyed += HandleChannelDestroyed;
}
public Task HandleChannelDestroyed(SocketChannel channel)
{
    Console.WriteLine($"A new channel '{channel.Name}'({channel.Id}, {channel.GetType()}) has been deleted.");
    return Task.CompletedTask;
}
#endregion

#region ChannelUpdated
public void HookChannelUpdated(BaseSocketClient client)
{
    client.ChannelUpdated += HandleChannelRename;
}
public Task HandleChannelRename(SocketChannel beforeChannel, SocketChannel afterChannel)
{
    if (beforeChannel.Name != afterChannel.Name)
        Console.WriteLine($"A channel ({beforeChannel.Id}) is renamed from {beforeChannel.Name} to {afterChannel.Name}.");
    return Task.CompletedTask;
}
#endregion

#region MessageReceived
private readonly ulong[] _targetUserIds = new {168693960628371456, 53905483156684800};
public void HookMessageReceived(BaseSocketClient client)
{
    client.MessageReceived += HandleMessageReceived;
}
public Task HandleMessageReceived(SocketMessage message)
{
    // check if the message is a user message as opposed to a system message (e.g. Clyde, pins, etc.)
    if (!(message is SocketUserMessage userMessage)) return;
    // check if the message origin is a guild message channel
    if (!(userMessage.Channel is SocketTextChannel textChannel)) return;
    // check if the target user was mentioned
    var targetUsers = userMessage.MentionedUsers.Where(x => _targetUserIds.Contains(x));
    if (targetUsers == null) return;
    foreach (var targetUser in targetUsers)
        Console.WriteLine($"{targetUser} was mentioned in the message '{message.Content}' by {message.Author}.");
    return Task.CompletedTask;
}
#endregion

#region MessageDeleted
public void HookMessageDeleted(BaseSocketClient client)
{
    client.MessageDeleted += HandleMessageDelete;
}
public Task HandleMessageDelete(Cacheable<IMessage, ulong> cachedMessage, ISocketMessageChannel channel)
{
    // check if the message exists in cache; if not, we cannot report what was removed
    if (!cachedMessage.HasValue) return;
    var message = cachedMessage.Value;
    Console.WriteLine($"A message ({message.Id}) from {message.Author} was removed from the channel {channel.Name} ({channel.Id}):"
        + Environment.NewLine
        + message.Content);
    return Task.CompletedTask;
}
#endregion