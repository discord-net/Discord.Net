using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System.Diagnostics;

namespace Audio;

internal class Program
{
    private static DiscordSocketClient _client;

    static async Task Main(string[] args)
    {
        await Utils.DownloadBinariesAsync();

        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged,
            EnableVoiceDaveEncryption = true
        });

        _client.Ready += OnReadyAsync;

        await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("token"));
        await _client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private static async Task OnReadyAsync()
    {
        SocketGuild guild = _client.GetGuild(ulong.Parse(Environment.GetEnvironmentVariable("guildId")));
        SocketVoiceChannel voiceChannel = guild.GetVoiceChannel(ulong.Parse(Environment.GetEnvironmentVariable("channelId")));
        IAudioClient audioClient = await voiceChannel.ConnectAsync(selfDeaf: false, selfMute: false, external: false);

        audioClient.Connected += () =>
        {
            Console.WriteLine("[Audio] Connected");
            return Task.CompletedTask;
        };

        audioClient.Disconnected += (ex) =>
        {
            Console.WriteLine($"[Audio] Disconnected: {ex.Message}");
            return Task.CompletedTask;
        };

        audioClient.StreamCreated += (userId, stream) =>
        {
            Console.WriteLine($"[Audio] Stream Created for User {userId}");
            return Task.CompletedTask;
        };

        audioClient.StreamDestroyed += (userId) =>
        {
            Console.WriteLine($"[Audio] Stream Destroyed for User {userId}");
            return Task.CompletedTask;
        };

        audioClient.SpeakingUpdated += (userId, speaking) =>
        {
            Console.WriteLine($"[Audio] Speaking Updated: User {userId} = {speaking}");
            return Task.CompletedTask;
        };

        await PlayAudioAsync(audioClient);
    }

    private static async Task PlayAudioAsync(IAudioClient audioClient)
    {
        const string audioUrl = "https://dn720306.ca.archive.org/0/items/S8_18/Gotye%20-%20Somebody%20That%20I%20Used%20To%20Know%20%28feat.%20Kimbra%29%20-%20official%20video.mp3";
        Process ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = Utils.FfmpegFileName,
            Arguments = $"-i {audioUrl} -ac 2 -f s16le -ar 48000 pipe:1",
            RedirectStandardOutput = true,
            CreateNoWindow = true
        })!;

        AudioOutStream audioStream = audioClient.CreatePCMStream(AudioApplication.Music);
        await ffmpeg.StandardOutput.BaseStream.CopyToAsync(audioStream);
        await audioStream.FlushAsync();
    }
}
