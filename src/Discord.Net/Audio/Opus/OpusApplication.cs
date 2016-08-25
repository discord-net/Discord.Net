namespace Discord.Audio
{
    /// <summary> The types of encoding which Opus supports during encoding. </summary>
    public enum OpusApplication : int
    {
        /// <summary> Specifies that the <see cref="Discord.Audio.IAudioClient"/> uses
        /// encoding to improve the quality of voice communication. </summary>
        Voice = 2048,
        /// <summary> Specifies that the <see cref="Discord.Audio.IAudioClient"/> uses
        /// encoding to improve the overall quality of mixed-media audio transmission. </summary>
        MusicOrMixed = 2049,
        /// <summary> Specifies that the <see cref="Discord.Audio.IAudioClient"/> uses
        /// encoding to reduce overall latency. </summary>
        LowLatency = 2051
    }
}
