using Discord.Audio;
using System;
using System.Threading.Tasks;

namespace Discord
{
    public partial class DiscordClient
	{
		public IDiscordVoiceClient GetVoiceClient(Server server)
		{
			if (server.Id <= 0) throw new ArgumentOutOfRangeException(nameof(server.Id));

			if (!Config.EnableVoiceMultiserver)
			{
				if (server.Id == _voiceServerId)
					return this;
				else
					return null;
            }

			DiscordWSClient client;
			if (_voiceClients.TryGetValue(server.Id, out client))
				return client;
			else
				return null;
		}
		private async Task<IDiscordVoiceClient> CreateVoiceClient(Server server)
		{
			if (!Config.EnableVoiceMultiserver)
			{
				_voiceServerId = server.Id;
				return this;
			}

			var client = _voiceClients.GetOrAdd(server.Id, _ =>
			{
				var config = _config.Clone();
				config.LogLevel = _config.LogLevel;// (LogMessageSeverity)Math.Min((int)_config.LogLevel, (int)LogMessageSeverity.Warning);
				config.VoiceOnly = true;
				config.VoiceClientId = unchecked(++_nextVoiceClientId);
				return new DiscordWSClient(config, server.Id);
			});
			client.LogMessage += (s, e) => RaiseOnLog(e.Severity, e.Source, $"(#{client.Config.VoiceClientId}) {e.Message}", e.Exception);
			await client.Connect(_gateway, _token).ConfigureAwait(false);
			return client;
		}

		public async Task<IDiscordVoiceClient> JoinVoiceServer(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			CheckReady(true); //checkVoice is done inside the voice client

			var client = await CreateVoiceClient(channel.Server).ConfigureAwait(false);
			await client.JoinChannel(channel.Id).ConfigureAwait(false);
			return client;
		}

		public async Task LeaveVoiceServer(Server server)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));			

			if (Config.EnableVoiceMultiserver)
			{
				//client.CheckReady();
				DiscordWSClient client;
				if (_voiceClients.TryRemove(server.Id, out client))
					await client.Disconnect().ConfigureAwait(false);
			}
			else
			{
				CheckReady(checkVoice: true);
				await _voiceSocket.Disconnect().ConfigureAwait(false);
				_dataSocket.SendLeaveVoice(server.Id);
			}
		}
	}
}
