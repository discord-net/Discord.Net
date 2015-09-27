using Discord.Helpers;
using Discord.WebSockets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    public partial class DiscordClient
	{
        public Task JoinVoiceServer(Channel channel)
			=> JoinVoiceServer(channel?.ServerId, channel?.Id);
		public Task JoinVoiceServer(Server server, string channelId)
			=> JoinVoiceServer(server?.Id, channelId);
		public async Task JoinVoiceServer(string serverId, string channelId)
		{
			CheckReady(checkVoice: true);
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			await LeaveVoiceServer().ConfigureAwait(false);
			_voiceSocket.SetChannel(serverId, channelId);
			_dataSocket.SendJoinVoice(serverId, channelId);

			CancellationTokenSource tokenSource = new CancellationTokenSource();
			try
			{
				await Task.Run(() =>  _voiceSocket.WaitForConnection(tokenSource.Token))
					.Timeout(_config.ConnectionTimeout, tokenSource)
					.ConfigureAwait(false);
			}
			catch (TimeoutException)
			{
				tokenSource.Cancel();
				await LeaveVoiceServer().ConfigureAwait(false);
				throw;
			}
		}
		public async Task LeaveVoiceServer()
		{
			CheckReady(checkVoice: true);

			if (_voiceSocket.State != WebSocketState.Disconnected)
			{
				var serverId = _voiceSocket.CurrentServerId;
				if (serverId != null)
				{
					await _voiceSocket.Disconnect().ConfigureAwait(false);
					_dataSocket.SendLeaveVoice(serverId);
				}
			}
		}

		/// <summary> Sends a PCM frame to the voice server. Will block until space frees up in the outgoing buffer. </summary>
		/// <param name="data">PCM frame to send. This must be a single or collection of uncompressed 48Kz monochannel 20ms PCM frames. </param>
		/// <param name="count">Number of bytes in this frame. </param>
		public void SendVoicePCM(byte[] data, int count)
		{
			CheckReady(checkVoice: true);
			if (data == null) throw new ArgumentException(nameof(data));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
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

			_voiceSocket.WaitForQueue();
			await TaskHelper.CompletedTask.ConfigureAwait(false);
		}
	}
}
