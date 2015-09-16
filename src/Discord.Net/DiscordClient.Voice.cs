using Discord.Helpers;
using System;
using System.Threading.Tasks;

namespace Discord
{
    public partial class DiscordClient
	{
		public Task JoinVoiceServer(string channelId)
			=> JoinVoiceServer(_channels[channelId]);
		public async Task JoinVoiceServer(Channel channel)
		{
			CheckReady(checkVoice: true);
			if (channel == null) throw new ArgumentNullException(nameof(channel));

			await LeaveVoiceServer().ConfigureAwait(false);
			_dataSocket.SendJoinVoice(channel);
			//await _voiceSocket.WaitForConnection().ConfigureAwait(false);
			//TODO: Add another ManualResetSlim to wait on here, base it off of DiscordClient's setup
		}

		public async Task LeaveVoiceServer()
		{
			CheckReady(checkVoice: true);

			if (_voiceSocket.CurrentVoiceServerId != null)
			{
				await _voiceSocket.Disconnect().ConfigureAwait(false);
				await TaskHelper.CompletedTask.ConfigureAwait(false);
				_dataSocket.SendLeaveVoice();
			}
		}

		/// <summary> Sends a PCM frame to the voice server. </summary>
		/// <param name="data">PCM frame to send. This must be a single or collection of uncompressed 48Kz monochannel 20ms PCM frames. </param>
		/// <param name="count">Number of bytes in this frame. </param>
		/// <remarks>Will block until</remarks>
		public void SendVoicePCM(byte[] data, int count)
		{
			CheckReady(checkVoice: true);
			if (count == 0) return;
			_voiceSocket.SendPCMFrames(data, count);
		}

		/// <summary> Clears the PCM buffer. </summary>
		public void ClearVoicePCM()
		{
			CheckReady(checkVoice: true);
			_voiceSocket.ClearPCMFrames();
		}

		/// <summary> Returns a task that completes once the voice output buffer is empty. </summary>
		public async Task WaitVoice()
		{
			CheckReady(checkVoice: true);
			_voiceSocket.Wait();
			await TaskHelper.CompletedTask.ConfigureAwait(false);
		}
	}
}
