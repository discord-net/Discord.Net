using Discord.Helpers;
using Discord.WebSockets;
using System;
using System.Threading.Tasks;

namespace Discord
{
    public partial class DiscordClient
	{
        public Task JoinVoiceServer(Channel channel)
			=> JoinVoiceServer(channel?.Server, channel);
		public Task JoinVoiceServer(string serverId, string channelId)
			=> JoinVoiceServer(_servers[serverId], _channels[channelId]);
		public Task JoinVoiceServer(Server server, string channelId)
			=> JoinVoiceServer(server, _channels[channelId]);
		private async Task JoinVoiceServer(Server server, Channel channel)
		{
			CheckReady(checkVoice: true);
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (channel == null) throw new ArgumentNullException(nameof(channel));

			await LeaveVoiceServer().ConfigureAwait(false);
			_voiceSocket.SetChannel(server, channel);
			_dataSocket.SendJoinVoice(server.Id, channel.Id);

			try
			{
				await Task.Run(() =>  _voiceSocket.WaitForConnection())
					.Timeout(_config.ConnectionTimeout)
					.ConfigureAwait(false);
			}
			catch (TaskCanceledException)
			{
				await LeaveVoiceServer().ConfigureAwait(false);
			}
		}
		public async Task LeaveVoiceServer()
		{
			CheckReady(checkVoice: true);

			if (_voiceSocket.State != WebSocketState.Disconnected)
			{
				var server = _voiceSocket.CurrentVoiceServer;
				if (server != null)
				{
					await _voiceSocket.Disconnect().ConfigureAwait(false);
					_dataSocket.SendLeaveVoice(server.Id);
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
