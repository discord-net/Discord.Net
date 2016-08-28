// Create an IAudioClient, and store it for later use
private IAudioClient _audio;

// Create a Join command, that will join the parameter or the user's current voice channel
[Command("join")]
public async Task JoinChannel(IUserMessage msg,
    IVoiceChannel channel = null)
{
    // Get the audio channel
    channel = channel ?? (msg.Author as IGuildUser)?.VoiceChannel;
    if (channel == null) { await msg.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

    // Get the IAudioClient by calling the JoinAsync method
    _audio = await channel.JoinAsync();
}