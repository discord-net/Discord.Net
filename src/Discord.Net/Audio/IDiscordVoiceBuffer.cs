namespace Discord.Audio
{
	public interface IDiscordVoiceBuffer
	{
		int FrameSize { get; }
		int FrameCount { get; }
		ushort ReadPos { get; }
		ushort WritePos { get; }
	}
}
