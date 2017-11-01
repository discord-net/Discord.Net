private async Task SendAsync(IAudioClient client, string path)
{
    // Create FFmpeg using the previous example
    using (var ffmpeg = CreateStream(path))
    using (var output = ffmpeg.StandardOutput.BaseStream)
    using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
    {
        try { await output.CopyToAsync(discord); }
        finally { await discord.FlushAsync(); }
    }
}
