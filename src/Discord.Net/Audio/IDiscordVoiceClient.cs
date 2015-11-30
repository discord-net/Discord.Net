using System.Threading.Tasks;

namespace Discord.Audio
{
	public interface IDiscordVoiceClient
	{
		long? ChannelId { get; }
		long? ServerId { get; }
		IDiscordVoiceBuffer OutputBuffer { get; }

		Task JoinChannel(long channelId);

		void SendVoicePCM(byte[] data, int count);
		void ClearVoicePCM();

		Task WaitVoice();
	}
}
