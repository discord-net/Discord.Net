using Discord;
using Discord.Audio;
using Discord.WebSocket;

namespace Audio;

internal class Program
{
    public enum AudioMode
    {
        Playback,
        Recording
    }

    private static DiscordSocketClient _client = null!;
    private static AudioMode _audioMode;

    static async Task Main(string[] args)
    {
        _audioMode = SelectMode();

        AudioSetupHandler audioSetup = new AudioSetupHandler();

        if (!await audioSetup.PrepareAsync())
        {
            Console.WriteLine("The setup process was cancelled by the user. The files required to continue are not available, so execution will be terminated.");
            return;
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

    private static AudioMode SelectMode()
    {
        Console.WriteLine("Select the audio mode:");
        Console.WriteLine("[1] Play");
        Console.WriteLine("[2] Record");

        ConsoleKey key = Console.ReadKey(true).Key;

        return key switch
        {
            ConsoleKey.D1 or ConsoleKey.NumPad1 => AudioMode.Playback,
            ConsoleKey.D2 or ConsoleKey.NumPad2 => AudioMode.Recording,
            _ => AudioMode.Playback
        };
    }

    private static async Task OnReadyAsync()
    {
        if (!ulong.TryParse(Environment.GetEnvironmentVariable("guildId"), out ulong guildId))
            throw new Exception("Guild id was not found");

        if (!ulong.TryParse(Environment.GetEnvironmentVariable("channelId"), out ulong channelId))
            throw new Exception("Channel id was not found");

        SocketGuild guild = _client.GetGuild(guildId);
        SocketVoiceChannel voiceChannel = guild.GetVoiceChannel(channelId);
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

        await StartAsync(voiceChannel, audioClient);
    }

    private static async Task StartAsync(SocketVoiceChannel voiceChannel, IAudioClient audioClient)
    {
        switch (_audioMode)
        {
            case AudioMode.Playback:
                Player player = new Player();
                await player.PlayAsync(audioClient);
                break;

            case AudioMode.Recording:
                Recorder recorder = new Recorder();
                await recorder.RecordAsync(voiceChannel, audioClient);
                break;
        }
    }
}
