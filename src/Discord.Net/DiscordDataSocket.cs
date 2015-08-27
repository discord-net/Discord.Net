using Discord.API.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebSocketMessage = Discord.API.Models.TextWebSocketCommands.WebSocketMessage;

namespace Discord
{
	internal sealed partial class DiscordDataSocket : DiscordWebSocket
	{
		private readonly ManualResetEventSlim _connectWaitOnLogin, _connectWaitOnLogin2;

		public DiscordDataSocket(DiscordClient client, int timeout, int interval, bool isDebug)
			: base(client, timeout, interval, isDebug)
		{
			_connectWaitOnLogin = new ManualResetEventSlim(false);
			_connectWaitOnLogin2 = new ManualResetEventSlim(false);
        }

		public async Task Login(string token)
		{
			var cancelToken = _disconnectToken.Token;

			_connectWaitOnLogin.Reset();
			_connectWaitOnLogin2.Reset();

			TextWebSocketCommands.Login msg = new TextWebSocketCommands.Login();
			msg.Payload.Token = token;
			msg.Payload.Properties["$os"] = "";
			msg.Payload.Properties["$browser"] = "";
			msg.Payload.Properties["$device"] = "Discord.Net";
			msg.Payload.Properties["$referrer"] = "";
			msg.Payload.Properties["$referring_domain"] = "";
			await SendMessage(msg, cancelToken);

			try
			{
				if (!_connectWaitOnLogin.Wait(_timeout, cancelToken)) //Waiting on READY message
					throw new Exception("No reply from Discord server");
			}
			catch (OperationCanceledException)
			{
				throw new InvalidOperationException("Bad Token");
			}
			try { _connectWaitOnLogin2.Wait(cancelToken); } //Waiting on READY handler
			catch (OperationCanceledException) { return; }

			SetConnected();
		}

		protected override Task ProcessMessage(string json)
		{
			var msg = JsonConvert.DeserializeObject<WebSocketMessage>(json);
			switch (msg.Operation)
			{
				case 0:
					{
						if (msg.Type == "READY")
						{
							var payload = (msg.Payload as JToken).ToObject<TextWebSocketEvents.Ready>();
							_heartbeatInterval = payload.HeartbeatInterval;
							QueueMessage(new TextWebSocketCommands.UpdateStatus());
							//QueueMessage(GetKeepAlive());
							_connectWaitOnLogin.Set(); //Pre-Event
						}
						RaiseGotEvent(msg.Type, msg.Payload as JToken);
						if (msg.Type == "READY")
							_connectWaitOnLogin2.Set(); //Post-Event
					}
					break;
				default:
					if (_isDebug)
						RaiseOnDebugMessage(DebugMessageType.WebSocketUnknownOpCode, "Unknown DataSocket op: " + msg.Operation);
					break;
			}
#if DNXCORE
			return Task.CompletedTask
#else
			return Task.Delay(0);
#endif
		}

		protected override object GetKeepAlive()
		{
			return new TextWebSocketCommands.KeepAlive();
        }

		public void JoinVoice(string serverId, string channelId)
		{
			var joinVoice = new TextWebSocketCommands.JoinVoice();
			joinVoice.Payload.ServerId = serverId;
			joinVoice.Payload.ChannelId = channelId;
            QueueMessage(joinVoice);
		}
		public void LeaveVoice()
		{
			var joinVoice = new TextWebSocketCommands.JoinVoice();
			QueueMessage(joinVoice);
		}
	}
}
