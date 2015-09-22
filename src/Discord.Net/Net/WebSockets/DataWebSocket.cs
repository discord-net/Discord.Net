using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    internal partial class DataWebSocket : WebSocket
    {
		private int _lastSeq;

		public string SessionId => _sessionId;
		private string _sessionId;

		public DataWebSocket(DiscordClient client)
			: base(client)
		{
		}

        public async Task Login(string token)
		{
			await Connect();
			
			Commands.Login msg = new Commands.Login();
			msg.Payload.Token = token;
			msg.Payload.Properties["$device"] = "Discord.Net";
			QueueMessage(msg);
        }
		private async Task Redirect(string server)
		{
			await DisconnectInternal(isUnexpected: false);
			await Connect();

			var resumeMsg = new Commands.Resume();
			resumeMsg.Payload.SessionId = _sessionId;
			resumeMsg.Payload.Sequence = _lastSeq;
			QueueMessage(resumeMsg);
		}

		protected override async Task ProcessMessage(string json)
		{
			var msg = JsonConvert.DeserializeObject<WebSocketMessage>(json);
			if (msg.Sequence.HasValue)
				_lastSeq = msg.Sequence.Value;
			
			switch (msg.Operation)
			{
				case 0:
					{
						JToken token = msg.Payload as JToken;
						if (msg.Type == "READY")
						{
							var payload = token.ToObject<Events.Ready>();
							_sessionId = payload.SessionId;
							_heartbeatInterval = payload.HeartbeatInterval;
							QueueMessage(new Commands.UpdateStatus());
						}
						else if (msg.Type == "RESUMED")
						{
							var payload = token.ToObject<Events.Resumed>();
							_heartbeatInterval = payload.HeartbeatInterval;
							QueueMessage(new Commands.UpdateStatus());
						}
						RaiseReceivedEvent(msg.Type, token);
						if (msg.Type == "READY" || msg.Type == "RESUMED")
							CompleteConnect();
					}
					break;
				case 7: //Redirect
					{
						var payload = (msg.Payload as JToken).ToObject<Events.Redirect>();
						Host = payload.Url;
						if (_logLevel >= LogMessageSeverity.Info)
							RaiseOnLog(LogMessageSeverity.Info, "Redirected to " + payload.Url);
						await Redirect(payload.Url);
					}
					break;
				default:
					if (_logLevel >= LogMessageSeverity.Warning)
						RaiseOnLog(LogMessageSeverity.Warning, $"Unknown Opcode: {msg.Operation}");
					break;
			}
		}

		protected override object GetKeepAlive()
		{
			return new Commands.KeepAlive();
		}

		public void SendJoinVoice(Channel channel)
		{
			var joinVoice = new Commands.JoinVoice();
			joinVoice.Payload.ServerId = channel.ServerId;
			joinVoice.Payload.ChannelId = channel.Id;
			QueueMessage(joinVoice);
		}
		public void SendLeaveVoice()
		{
			var leaveVoice = new Commands.JoinVoice();
			QueueMessage(leaveVoice);
		}
	}
}
