using Discord.API.Models;
using Discord.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed partial class DiscordTextWebSocket : DiscordWebSocket
	{
		private const int ReadyTimeout = 2500; //Max time in milliseconds between connecting to Discord and receiving a READY event

		private ManualResetEventSlim _connectWaitOnLogin, _connectWaitOnLogin2;

		public DiscordTextWebSocket(int interval)
			: base(interval)
		{
			_connectWaitOnLogin = new ManualResetEventSlim(false);
			_connectWaitOnLogin2 = new ManualResetEventSlim(false);
		}

		public async Task Login()
		{
			var cancelToken = _disconnectToken.Token;

			_connectWaitOnLogin.Reset();
			_connectWaitOnLogin2.Reset();

			TextWebSocketCommands.Login msg = new TextWebSocketCommands.Login();
			msg.Payload.Token = Http.Token;
			msg.Payload.Properties["$os"] = "";
			msg.Payload.Properties["$browser"] = "";
			msg.Payload.Properties["$device"] = "Discord.Net";
			msg.Payload.Properties["$referrer"] = "";
			msg.Payload.Properties["$referring_domain"] = "";
			await SendMessage(msg, cancelToken);

			try
			{
				if (!_connectWaitOnLogin.Wait(ReadyTimeout, cancelToken)) //Waiting on READY message
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

		protected override void ProcessMessage(WebSocketMessage msg)
		{
			switch (msg.Operation)
			{
				case 0:
					if (msg.Type == "READY")
					{
						var payload = (msg.Payload as JToken).ToObject<TextWebSocketEvents.Ready>();
						_heartbeatInterval = payload.HeartbeatInterval;
						QueueMessage(new TextWebSocketCommands.UpdateStatus());
						QueueMessage(new TextWebSocketCommands.KeepAlive());
						_connectWaitOnLogin.Set(); //Pre-Event
					}
					RaiseGotEvent(msg.Type, msg.Payload as JToken);
					if (msg.Type == "READY")
						_connectWaitOnLogin2.Set(); //Post-Event
					break;
				default:
					RaiseOnDebugMessage("Unknown WebSocket operation ID: " + msg.Operation);
					break;
			}
		}

		protected override WebSocketMessage GetKeepAlive()
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
