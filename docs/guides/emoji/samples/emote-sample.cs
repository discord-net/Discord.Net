public async Task ReactWithEmoteAsync(SocketUserMessage userMsg, string escapedEmote)
{
    if (Emote.TryParse(escapedEmote, out var emote))
    {
        await userMsg.AddReactionAsync(emote);
    }
}