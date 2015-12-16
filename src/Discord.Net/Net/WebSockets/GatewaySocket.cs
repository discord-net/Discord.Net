using Discord.API.Client;
using Discord.API.Client.GatewaySocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public partial class GatewaySocket : WebSocket
	{
		public int LastSequence => _lastSeq;
		private int _lastSeq;

		public string SessionId => _sessionId;
		private string _sessionId;

		public GatewaySocket(DiscordClient client, Logger logger)
			: base(client, logger)
		{
			Disconnected += async (s, e) =>
			{
				if (e.WasUnexpected)
					await Reconnect().ConfigureAwait(false);
			};
		}

        public async Task Connect()
        {
			await BeginConnect().ConfigureAwait(false);
			SendIdentify();
        }
		private async Task Redirect()
        {
            await BeginConnect().ConfigureAwait(false);
			SendResume();
		}
		private async Task Reconnect()
		{
			try
			{
				var cancelToken = ParentCancelToken.Value;
				await Task.Delay(_client.Config.ReconnectDelay, cancelToken).ConfigureAwait(false);
				while (!cancelToken.IsCancellationRequested)
				{
					try
                    {
                        await Connect().ConfigureAwait(false);
						break;
					}
					catch (OperationCanceledException) { throw; }
					catch (Exception ex)
					{
						_logger.Log(LogSeverity.Error, $"Reconnect failed", ex);
						//Net is down? We can keep trying to reconnect until the user runs Disconnect()
						await Task.Delay(_client.Config.FailedReconnectDelay, cancelToken).ConfigureAwait(false);
					}
				}
			}
			catch (OperationCanceledException) { }
		}
        public Task Disconnect() => TaskManager.Stop();

		protected override async Task Run()
        {
            List<Task> tasks = new List<Task>();
            tasks.AddRange(_engine.GetTasks(_cancelToken));
            tasks.Add(HeartbeatAsync(_cancelToken));
            await _taskManager.Start(tasks, _cancelTokenSource).ConfigureAwait(false);
        }

		protected override async Task ProcessMessage(string json)
		{
			await base.ProcessMessage(json).ConfigureAwait(false);
			var msg = JsonConvert.DeserializeObject<WebSocketMessage>(json);
			if (msg.Sequence.HasValue)
				_lastSeq = msg.Sequence.Value;

			var opCode = (OpCodes)msg.Operation;
            switch (opCode)
			{
				case OpCodes.Dispatch:
					{
						JToken token = msg.Payload as JToken;
						if (msg.Type == "READY")
						{
							var payload = token.ToObject<ReadyEvent>(_serializer);
							_sessionId = payload.SessionId;
							_heartbeatInterval = payload.HeartbeatInterval;
						}
						else if (msg.Type == "RESUMED")
						{
							var payload = token.ToObject<ResumedEvent>(_serializer);
							_heartbeatInterval = payload.HeartbeatInterval;
						}
						RaiseReceivedDispatch(msg.Type, token);
						if (msg.Type == "READY" || msg.Type == "RESUMED")
							EndConnect(); //Complete the connect
					}
					break;
				case OpCodes.Redirect:
					{
						var payload = (msg.Payload as JToken).ToObject<RedirectEvent>(_serializer);
						if (payload.Url != null)
						{
							Host = payload.Url;
							if (_logger.Level >= LogSeverity.Info)
								_logger.Info("Redirected to " + payload.Url);
							await Redirect().ConfigureAwait(false);
						}
					}
					break;
				default:
					if (_logger.Level >= LogSeverity.Warning)
						_logger.Log(LogSeverity.Warning, $"Unknown Opcode: {opCode}");
					break;
			}
		}

        public void SendIdentify()
        {
            var props = new Dictionary<string, string>
            {
                ["$device"] = "Discord.Net"
            };
            var msg = new IdentifyCommand()
            {
                Version = 3,
                Token = _client.Token,
                Properties = props, 
                LargeThreshold = _client.Config.UseLargeThreshold ? 100 : (int?)null,
                UseCompression = true
            };
			QueueMessage(msg);
		}

        public void SendResume()
            => QueueMessage(new ResumeCommand { SessionId = _sessionId, Sequence = _lastSeq });
		public override void SendHeartbeat() 
            => QueueMessage(new HeartbeatCommand());
		public void SendUpdateStatus(long? idleSince, int? gameId) 
            => QueueMessage(new UpdateStatusCommand { IdleSince = idleSince, GameId = gameId });
		public void SendUpdateVoice(ulong serverId, ulong channelId, bool isSelfMuted, bool isSelfDeafened)
            => QueueMessage(new UpdateVoiceCommand { GuildId = serverId, ChannelId = channelId, IsSelfMuted = isSelfMuted, IsSelfDeafened = isSelfDeafened });
		public void SendRequestMembers(ulong serverId, string query, int limit)
            => QueueMessage(new RequestMembersCommand { GuildId = serverId, Query = query, Limit = limit });
	}
}
