using Discord.Audio;
using System;
using System.Threading.Tasks;

namespace Discord
{
    public partial class DiscordClient
	{
		public IDiscordVoiceClient GetVoiceClient(Server server)
			=> GetVoiceClient(server.Id);
		public IDiscordVoiceClient GetVoiceClient(string serverId)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));

			if (!Config.EnableVoiceMultiserver)
			{
				if (serverId == _voiceServerId)
					return this;
				else
					return null;
			}

			DiscordWSClient client;
			if (_voiceClients.TryGetValue(serverId, out client))
				return client;
			else
				return null;
		}
		private async Task<IDiscordVoiceClient> CreateVoiceClient(string serverId)
		{
			if (!Config.EnableVoiceMultiserver)
			{
				_voiceServerId = serverId;
				return this;
			}

			var client = _voiceClients.GetOrAdd(serverId, _ =>
			{
				var config = _config.Clone();
				config.LogLevel = _config.LogLevel;// (LogMessageSeverity)Math.Min((int)_config.LogLevel, (int)LogMessageSeverity.Warning);
				config.VoiceOnly = true;
				config.VoiceClientId = unchecked(++_nextVoiceClientId);
				return new DiscordWSClient(config, serverId);
			});
			client.LogMessage += (s, e) => RaiseOnLog(e.Severity, e.Source, $"(#{client.Config.VoiceClientId}) {e.Message}");
			await client.Connect(_gateway, _token).ConfigureAwait(false);
			return client;
		}

		public async Task<IDiscordVoiceClient> JoinVoiceServer(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			CheckReady(); //checkVoice is done inside the voice client

			var client = await CreateVoiceClient(channel.Server.Id).ConfigureAwait(false);
			await client.JoinChannel(channel.Id).ConfigureAwait(false);
			return client;
		}

		public Task LeaveVoiceServer(Server server)
			=> LeaveVoiceServer(server?.Id);
		public async Task LeaveVoiceServer(string serverId)
		{
			CheckReady(checkVoice: true);
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));

			if (Config.EnableVoiceMultiserver)
			{
				DiscordWSClient client;
				if (_voiceClients.TryRemove(serverId, out client))
					await client.Disconnect().ConfigureAwait(false);
			}
			else
			{
				await _voiceSocket.Disconnect().ConfigureAwait(false);
				_dataSocket.SendLeaveVoice(serverId);
			}
		}
	}
}
