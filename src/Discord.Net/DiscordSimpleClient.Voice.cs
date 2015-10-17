using Discord.Helpers;
using Discord.WebSockets;
using Discord.WebSockets.Voice;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
	public interface IDiscordVoiceBuffer
	{
		int FrameSize { get; }
		int FrameCount { get; }
		ushort ReadPos { get; }
		ushort WritePos { get; }
	}

	public interface IDiscordVoiceClient
	{
		IDiscordVoiceBuffer OutputBuffer { get; }

		Task JoinChannel(string channelId);

        void SendVoicePCM(byte[] data, int count);
		void ClearVoicePCM();

		Task WaitVoice();
	}

	public partial class DiscordSimpleClient : IDiscordVoiceClient
	{
		IDiscordVoiceBuffer IDiscordVoiceClient.OutputBuffer => _voiceSocket.OutputBuffer;

		async Task IDiscordVoiceClient.JoinChannel(string channelId)
		{
			CheckReady(checkVoice: true);
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			
			await _voiceSocket.Disconnect().ConfigureAwait(false);
			
			await _voiceSocket.SetChannel(_voiceServerId, channelId).ConfigureAwait(false);
			_dataSocket.SendJoinVoice(_voiceServerId, channelId);
			await _voiceSocket.WaitForConnection(_config.ConnectionTimeout);
		}

		/*async Task IDiscordVoiceClient.Disconnect()
		{
			CheckReady(checkVoice: true);

			if (_voiceSocket.State != WebSocketState.Disconnected)
			{
				if (_voiceSocket.CurrentServerId != null)
				{
					await _voiceSocket.Disconnect().ConfigureAwait(false);
					_dataSocket.SendLeaveVoice(_voiceSocket.CurrentServerId);
				}
			}
		}*/

		/// <summary> Sends a PCM frame to the voice server. Will block until space frees up in the outgoing buffer. </summary>
		/// <param name="data">PCM frame to send. This must be a single or collection of uncompressed 48Kz monochannel 20ms PCM frames. </param>
		/// <param name="count">Number of bytes in this frame. </param>
		void IDiscordVoiceClient.SendVoicePCM(byte[] data, int count)
		{
			CheckReady(checkVoice: true);
			if (data == null) throw new ArgumentException(nameof(data));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
			if (count == 0) return;
			
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
