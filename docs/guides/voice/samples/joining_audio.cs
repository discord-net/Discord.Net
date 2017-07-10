[Command("join")]
public async Task JoinChannel(IVoiceChannel channel = null)
{
    // Get the audio channel
    channel = channel ?? (msg.Author as IGuildUser)?.VoiceChannel;
    if (channel == null) { await msg.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

    // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
    var audioClient = await channel.ConnectAsync();
}