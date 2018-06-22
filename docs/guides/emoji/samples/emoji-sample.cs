public async Task SendEmojiAndEmote(ISocketMessageChannel channel)
{
    await channel.SendMessageAsync("\uD83D\uDC4C");
    // or
    // await channel.SendMessageAsync("👌");
}