using System.Threading.Tasks;

namespace Discord.Audio
{
	public interface IDiscordVoiceClient
	{
		IDiscordVoiceBuffer OutputBuffer { get; }

		Task JoinChannel(long channelId);

		void SendVoicePCM(byte[] data, int count);
		void ClearVoicePCM();

		Task WaitVoice();
	}
}
