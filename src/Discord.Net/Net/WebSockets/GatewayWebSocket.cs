using Discord.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public partial class GatewayWebSocket : WebSocket
	{
        private int _lastSeq;

		public string SessionId => _sessionId;
		private string _sessionId;

		public GatewayWebSocket(DiscordConfig config, Logger logger)
			: base(config, logger)
		{
		}

        public async Task Login(string token)
		{
			await BeginConnect().ConfigureAwait(false);
			await Start().ConfigureAwait(false);

			SendIdentify(token);
        }
		private async Task Redirect(string server)
		{
			await DisconnectInternal(isUnexpected: false).ConfigureAwait(false);

			await BeginConnect().ConfigureAwait(false);
			await Start().ConfigureAwait(false);

			SendResume();
		}
		public async Task Reconnect(string token)
		{
			try
			{
				var cancelToken = ParentCancelToken.Value;
				await Task.Delay(_config.ReconnectDelay, cancelToken).ConfigureAwait(false);
				while (!cancelToken.IsCancellationRequested)
				{
					try
					{
						await Login(token).ConfigureAwait(false);
						break;
					}
					catch (OperationCanceledException) { throw; }
					catch (Exception ex)
					{
						_logger.Log(LogSeverity.Error, $"Reconnect failed", ex);
						//Net is down? We can keep trying to reconnect until the user runs Disconnect()
						await Task.Delay(_config.FailedReconnectDelay, cancelToken).ConfigureAwait(false);
					}
				}
			}
			catch (OperationCanceledException) { }
		}

		protected override async Task ProcessMessage(string json)
		{
			await base.ProcessMessage(json).ConfigureAwait(false);
			var msg = JsonConvert.DeserializeObject<WebSocketMessage>(json);
			if (msg.Sequence.HasValue)
				_lastSeq = msg.Sequence.Value;

			var opCode = (GatewayOpCodes)msg.Operation;
            switch (opCode)
			{
				case GatewayOpCodes.Dispatch:
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
							var payload = token.ToObject<ResumeEvent>(_serializer);
							_heartbeatInterval = payload.HeartbeatInterval;
						}
						RaiseReceivedDispatch(msg.Type, token);
						if (msg.Type == "READY" || msg.Type == "RESUMED")
							EndConnect();
					}
					break;
				case GatewayOpCodes.Redirect:
					{
						var payload = (msg.Payload as JToken).ToObject<RedirectEvent>(_serializer);
						if (payload.Url != null)
						{
							Host = payload.Url;
							if (_logger.Level >= LogSeverity.Info)
								_logger.Log(LogSeverity.Info, "Redirected to " + payload.Url);
							await Redirect(payload.Url).ConfigureAwait(false);
						}
					}
					break;
				default:
					if (_logger.Level >= LogSeverity.Warning)
						_logger.Log(LogSeverity.Warning, $"Unknown Opcode: {opCode}");
					break;
			}
		}

		public void SendIdentify(string token)
		{
			IdentifyCommand msg = new IdentifyCommand();
			msg.Payload.Token = token;
			msg.Payload.Properties["$device"] = "Discord.Net";
			if (_config.UseLargeThreshold)
				msg.Payload.LargeThreshold = 100;
			msg.Payload.Compress = true;
			QueueMessage(msg);
		}

		public void SendResume()
		{
			var resumeMsg = new ResumeCommand();
			resumeMsg.Payload.SessionId = _sessionId;
			resumeMsg.Payload.Sequence = _lastSeq;
			QueueMessage(resumeMsg);
		}

		public override void SendHeartbeat()
		{
			QueueMessage(new HeartbeatCommand());
		}

		public void SendStatusUpdate(long? idleSince, int? gameId)
		{
			var updateStatus = new StatusUpdateCommand();
			updateStatus.Payload.IdleSince = idleSince;
			updateStatus.Payload.GameId = gameId;
            QueueMessage(updateStatus);
		}

		public void SendJoinVoice(long serverId, long channelId)
		{
			var joinVoice = new JoinVoiceCommand();
			joinVoice.Payload.ServerId = serverId;
			joinVoice.Payload.ChannelId = channelId;
			QueueMessage(joinVoice);
		}
		public void SendLeaveVoice(long serverId)
		{
			var leaveVoice = new JoinVoiceCommand();
			leaveVoice.Payload.ServerId = serverId;
			QueueMessage(leaveVoice);
		}

		public void SendRequestUsers(long serverId, string query = "", int limit = 0)
		{
			var getOfflineUsers = new GetUsersCommand();
			getOfflineUsers.Payload.ServerId = serverId;
			QueueMessage(getOfflineUsers);
		}
	}
}
