public async Task ReactAsync(SocketUserMessage userMsg)
{
    // equivalent to "👌"
    var emoji = new Emoji("\uD83D\uDC4C");
    await userMsg.AddReactionAsync(emoji);
}