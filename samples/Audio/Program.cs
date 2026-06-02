using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System.Diagnostics;

namespace Audio;

internal class Program
{
    private static DiscordSocketClient _client = null!;

    static async Task Main(string[] args)
    {
        using (AudioSetupHandler audioSetup = new AudioSetupHandler())
        {
            if (!await audioSetup.PrepareAsync())
            {
                Console.WriteLine("The setup process was cancelled by the user. The files required to continue are not available, so execution will be terminated.");
                return;
            }
        }

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
        const string audioUrl = "https://github.com/ShivamJoker/sample-songs/raw/refs/heads/master/Faded.mp3";
        Process ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i {audioUrl} -ac 2 -f s16le -ar 48000 pipe:1",
            RedirectStandardOutput = true,
            CreateNoWindow = true
        })!;

        AudioOutStream audioStream = audioClient.CreatePCMStream(AudioApplication.Music);
        await ffmpeg.StandardOutput.BaseStream.CopyToAsync(audioStream);
        await audioStream.FlushAsync();
    }
}
