using Discord.Audio;
using Discord.WebSocket;
using NAudio.Wave;
using System.Collections.Concurrent;

namespace Audio;

internal class Recorder
{
    private const string SavedFilename = "audio.wav";
    private static readonly TimeSpan RecordTime = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan AudioFrameDuration = TimeSpan.FromMilliseconds(250);

    private ConcurrentBag<UserVoiceData> _usersData = new ConcurrentBag<UserVoiceData>();
    private List<Task> _recordTasks = new List<Task>();
    private Lock _recordTasksLock = new Lock();
    private CancellationTokenSource _cancellation = new CancellationTokenSource();

    public async Task RecordAsync(SocketVoiceChannel voiceChannel, IAudioClient audioClient)
    {
        Console.WriteLine($"Start recording {RecordTime.TotalSeconds} seconds of audio");

        List<SocketGuildUser> voiceChannelUsers = voiceChannel.Users.ToList();

        audioClient.StreamCreated += UserStreamCreatedAsync;

        voiceChannelUsers.ForEach(user => UserStreamCreatedAsync(user.Id, user.AudioStream));

        await Task.Delay(RecordTime);

        audioClient.StreamCreated -= UserStreamCreatedAsync;
        _cancellation.Cancel();

        await Task.WhenAll(_recordTasks);
        await SaveAsync();
    }

    private Task UserStreamCreatedAsync(ulong userId, AudioInStream stream)
    {
        UserVoiceData? userVoiceData = _usersData.FirstOrDefault(user => user.Id == userId);

        if (userVoiceData is null)
        {
            userVoiceData = new UserVoiceData(userId);
            _usersData.Add(userVoiceData);
        }

        if (stream is not null)
        {
            Task task = RecordUserAsync(userVoiceData, stream, _cancellation.Token);

            lock (_recordTasksLock)
                _recordTasks.Add(task);

            task.ContinueWith(task =>
            {
                lock (_recordTasksLock)
                    _recordTasks.Remove(task);
            });
        }

        return Task.CompletedTask;
    }

    private async Task RecordUserAsync(UserVoiceData user, AudioInStream stream, CancellationToken cancellation)
    {
        long lastFrameTime = -1;

        try
        {
            while (!cancellation.IsCancellationRequested)
            {
                if (stream.AvailableFrames == 0)
                {
                    await Task.Delay(AudioFrameDuration);
                }
                else
                {
                    long now = DateTime.UtcNow.Ticks;
                    RTPFrame frame = await stream.ReadFrameAsync(cancellation);

                    if (now - lastFrameTime > AudioFrameDuration.Ticks)
                    {
                        VoiceData voiceData = new VoiceData(now);
                        voiceData.Frames.Add(frame);
                        user.VoicesData.Add(voiceData);
                    }
                    else
                    {
                        user.VoicesData[user.VoicesData.Count - 1].Frames.Add(frame);
                    }

                    lastFrameTime = now;
                }
            }
        }
        catch { }
    }

    private async Task SaveAsync()
    {
        List<VoiceData> voicesData = _usersData.SelectMany(user => user.VoicesData).ToList();

        if (voicesData.Count == 0)
        {
            Console.WriteLine("Nothing has been recorded");
            return;
        }

        long initialTime = voicesData.Min(voice => voice.Ticks);
        WaveFormat format = new WaveFormat(48000, 2);
        WaveMixerStream32 mixer = new WaveMixerStream32();

        foreach (VoiceData voiceData in voicesData)
        {
            long time = voiceData.Ticks - initialTime;
            MemoryStream framesStream = new MemoryStream();
            voiceData.Frames.ForEach(frame => framesStream.Write(frame.Payload));

            RawSourceWaveStream waveStream = new RawSourceWaveStream(framesStream, format)
            {
                Position = 0
            };

            WaveOffsetStream offsetStream = new WaveOffsetStream(waveStream)
            {
                StartTime = TimeSpan.FromTicks(time)
            };

            mixer.AddInputStream(new WaveChannel32(offsetStream));
        }

        using WaveFileWriter writer = new WaveFileWriter(SavedFilename, mixer.WaveFormat);
        mixer.Position = 0;
        await mixer.CopyToAsync(writer);

        Console.WriteLine($"Audio saved successfully as {SavedFilename}");
    }

    private class UserVoiceData
    {
        public ulong Id { get; }
        public List<VoiceData> VoicesData { get; }

        public UserVoiceData(ulong id)
        {
            Id = id;
            VoicesData = new List<VoiceData>();
        }
    }

    private class VoiceData
    {
        public long Ticks { get; }
        public List<RTPFrame> Frames { get; }

        public VoiceData(long tick)
        {
            Ticks = tick;
            Frames = new List<RTPFrame>();
        }
    }
}
