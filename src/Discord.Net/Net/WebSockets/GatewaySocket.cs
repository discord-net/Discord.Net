using Discord.API.Client;
using Discord.API.Client.GatewaySocket;
using Discord.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public sealed class WebSocketEventEventArgs : EventArgs
    {
        public readonly string Type;
        public readonly JToken Payload;
        internal WebSocketEventEventArgs(string type, JToken data)
        {
            Type = type;
            Payload = data;
        }
    }

    public partial class GatewaySocket : WebSocket
    {
        private int _lastSequence;
		private string _sessionId;
        private string _token;
        private int _reconnects;

        public string Token { get { return _token; } internal set { _token = value; _sessionId = null; } }

        public event EventHandler<WebSocketEventEventArgs> ReceivedDispatch = delegate { };
        private void OnReceivedDispatch(string type, JToken payload)
            => ReceivedDispatch(this, new WebSocketEventEventArgs(type, payload));

        public GatewaySocket(DiscordClient client, JsonSerializer serializer, Logger logger)
			: base(client, serializer, logger)
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
            if (_sessionId == null)
                SendIdentify(Token);
            else
                SendResume();
        }
		private async Task Reconnect()
		{
			try
			{
				var cancelToken = ParentCancelToken.Value;
                if (_reconnects++ == 0)
				    await Task.Delay(_client.Config.ReconnectDelay, cancelToken).ConfigureAwait(false);
                else
                    await Task.Delay(_client.Config.FailedReconnectDelay, cancelToken).ConfigureAwait(false);
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
						Logger.Error("Reconnect failed", ex);
						//Net is down? We can keep trying to reconnect until the user runs Disconnect()
						await Task.Delay(_client.Config.FailedReconnectDelay, cancelToken).ConfigureAwait(false);
					}
				}
			}
			catch (OperationCanceledException) { }
		}
        public Task Disconnect() => _taskManager.Stop(true);

		protected override async Task Run()
        {
            List<Task> tasks = new List<Task>();
            tasks.AddRange(_engine.GetTasks(CancelToken));
            tasks.Add(HeartbeatAsync(CancelToken));
            await _taskManager.Start(tasks, _cancelTokenSource).ConfigureAwait(false);
        }
        protected override Task Cleanup()
        {
            var ex = _taskManager.Exception;
            if (ex == null || (ex as WebSocketException)?.Code != 1012) //if (ex == null || (ex as WebSocketException)?.Code != 1012)
                _sessionId = null; //Reset session unless close code 1012
            return base.Cleanup();
        }

        protected override async Task ProcessMessage(string json)
		{
            base.ProcessMessage(json).Wait(); //This is just a CompletedTask, and we need to avoid asyncs in here
			var msg = JsonConvert.DeserializeObject<WebSocketMessage>(json);
			if (msg.Sequence.HasValue)
				_lastSequence = msg.Sequence.Value;

			var opCode = (OpCodes?)msg.Operation;
            switch (opCode)
			{
				case OpCodes.Dispatch:
					{
						JToken token = msg.Payload as JToken;
						if (msg.Type == "READY")
						{
                            _reconnects = 0;
                            var payload = token.ToObject<ReadyEvent>(_serializer);
							_sessionId = payload.SessionId;
                            _heartbeatInterval = payload.HeartbeatInterval;
						}
						else if (msg.Type == "RESUMED")
						{
							var payload = token.ToObject<ResumedEvent>(_serializer);
							_heartbeatInterval = payload.HeartbeatInterval;
						}
						OnReceivedDispatch(msg.Type, token);
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
							Logger.Info("Redirected to " + payload.Url);
							await Reconnect().ConfigureAwait(false);
						}
					}
					break;
				default:
                    if (opCode != null)
                        Logger.Warning($"Unknown Opcode: {opCode}");
                    else
                        Logger.Warning($"Received message with no opcode");
                    break;
			}
		}

        public void SendIdentify(string token)
        {
            var props = new Dictionary<string, string>
            {
                ["$device"] = "Discord.Net"
            };
            var msg = new IdentifyCommand()
            {
                Version = 3,
                Token = token,
                Properties = props, 
                LargeThreshold = _client.Config.UseLargeThreshold ? 100 : (int?)null,
                UseCompression = true
            };
			QueueMessage(msg);
		}

        public void SendResume()
            => QueueMessage(new ResumeCommand { SessionId = _sessionId, Sequence = _lastSequence });
		public override void SendHeartbeat() 
            => QueueMessage(new HeartbeatCommand());
		public void SendUpdateStatus(long? idleSince, string gameName) 
            => QueueMessage(new UpdateStatusCommand
            {
                IdleSince = idleSince,
                Game = gameName != null ? new UpdateStatusCommand.GameInfo { Name = gameName } : null
            });
		public void SendUpdateVoice(ulong serverId, ulong channelId, bool isSelfMuted, bool isSelfDeafened)
            => QueueMessage(new UpdateVoiceCommand { GuildId = serverId, ChannelId = channelId, IsSelfMuted = isSelfMuted, IsSelfDeafened = isSelfDeafened });
		public void SendRequestMembers(ulong serverId, string query, int limit)
            => QueueMessage(new RequestMembersCommand { GuildId = serverId, Query = query, Limit = limit });
	}
}
