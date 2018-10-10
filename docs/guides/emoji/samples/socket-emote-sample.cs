private readonly DiscordSocketClient _client;

public async Task ReactAsync(SocketUserMessage userMsg, string emoteName)
{
    var emote = _client.Guilds
            .SelectMany(x => x.Emotes)
            .FirstOrDefault(x => x.Name.IndexOf(
                emoteName, StringComparison.OrdinalIgnoreCase) != -1);
    if (emote == null) return;
    await userMsg.AddReactionAsync(emote);
}