// The command's Run Mode MUST be set to RunMode.Async, otherwise, being connected to a voice channel will block the gateway thread.
[Command("join", RunMode = RunMode.Async)] 
public async Task JoinChannel(IVoiceChannel channel = null)
{
    // Get the audio channel
    channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
    if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

    // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
    var audioClient = await channel.ConnectAsync();
}
