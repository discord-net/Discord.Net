using Discord.Audio;
using System;
using System.Threading.Tasks;

namespace Discord
{
	public partial class DiscordWSClient : IDiscordVoiceClient
	{
		IDiscordVoiceBuffer IDiscordVoiceClient.OutputBuffer => _voiceSocket.OutputBuffer;

		async Task IDiscordVoiceClient.JoinChannel(long channelId)
		{
			CheckReady(checkVoice: true);
			if (channelId <= 0) throw new ArgumentOutOfRangeException(nameof(channelId));
			
			await _voiceSocket.Disconnect().ConfigureAwait(false);
			
			await _voiceSocket.SetChannel(_voiceServerId.Value, channelId).ConfigureAwait(false);
			_dataSocket.SendJoinVoice(_voiceServerId.Value, channelId);
			await _voiceSocket.WaitForConnection(_config.ConnectionTimeout).ConfigureAwait(false);
		}

		/// <summary> Sends a PCM frame to the voice server. Will block until space frees up in the outgoing buffer. </summary>
		/// <param name="data">PCM frame to send. This must be a single or collection of uncompressed 48Kz monochannel 20ms PCM frames. </param>
		/// <param name="count">Number of bytes in this frame. </param>
		void IDiscordVoiceClient.SendVoicePCM(byte[] data, int count)
		{
			if (data == null) throw new ArgumentException(nameof(data));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
			CheckReady(checkVoice: true);

			if (count != 0)
				_voiceSocket.SendPCMFrames(data, count);
		}
		/// <summary> Clears the PCM buffer. </summary>
		void IDiscordVoiceClient.ClearVoicePCM()
		{
			CheckReady(checkVoice: true);

			_voiceSocket.ClearPCMFrames();
		}

		/// <summary> Returns a task that completes once the voice output buffer is empty. </summary>
		async Task IDiscordVoiceClient.WaitVoice()
		{
			CheckReady(checkVoice: true);

			_voiceSocket.WaitForQueue();
			await TaskHelper.CompletedTask.ConfigureAwait(false);
		}
	}
}