using Discord.Audio;
using System.Diagnostics;

namespace Audio;

internal class Player
{
    private const string AudioUrl = "https://github.com/ShivamJoker/sample-songs/raw/refs/heads/master/Faded.mp3";

    public async Task PlayAsync(IAudioClient audioClient)
    {
        Console.WriteLine("Start playing audio");

        Process ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i {AudioUrl} -ac 2 -f s16le -ar 48000 pipe:1",
            RedirectStandardOutput = true,
            CreateNoWindow = true
        })!;

        AudioOutStream audioStream = audioClient.CreatePCMStream(AudioApplication.Music);
        await ffmpeg.StandardOutput.BaseStream.CopyToAsync(audioStream);
        await audioStream.FlushAsync();
    }
}
