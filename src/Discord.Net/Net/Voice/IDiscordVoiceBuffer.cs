namespace Discord.Net.Voice
{
	public interface IDiscordVoiceBuffer
	{
		int FrameSize { get; }
		int FrameCount { get; }
		ushort ReadPos { get; }
		ushort WritePos { get; }
	}
}
