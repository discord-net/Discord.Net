using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    internal partial class DataWebSocket : WebSocket
    {		
		private string _redirectServer;
		private int _lastSeq;

		public string SessionId => _sessionId;
		private string _sessionId;

		public DataWebSocket(DiscordClient client)
			: base(client)
		{
		}
		
		public async Task Login(string host, string token)
		{
			await base.Connect(host);
			
			Commands.Login msg = new Commands.Login();
			msg.Payload.Token = token;
			msg.Payload.Properties["$device"] = "Discord.Net";
			QueueMessage(msg);
        }

		protected override Task[] Run()
		{
			//Send resume session if we were transferred
			if (_redirectServer != null)
			{
				var resumeMsg = new Commands.Resume();
				resumeMsg.Payload.SessionId = _sessionId;
				resumeMsg.Payload.Sequence = _lastSeq;
				QueueMessage(resumeMsg);
				_redirectServer = null;
			}
			return base.Run();
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
						RaiseReceivedEvent(msg.Type, token);
						if (msg.Type == "READY")
							CompleteConnect();
						/*if (_logLevel >= LogMessageSeverity.Info)
							RaiseOnLog(LogMessageSeverity.Info, "Got Event: " + msg.Type);*/
					}
					break;
				case 7: //Redirect
					{
						var payload = (msg.Payload as JToken).ToObject<Events.Redirect>();
						_host = payload.Url;
						if (_logLevel >= LogMessageSeverity.Info)
							RaiseOnLog(LogMessageSeverity.Info, "Redirected to " + payload.Url);
						await DisconnectInternal(new Exception("Server is redirecting."), true);
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
