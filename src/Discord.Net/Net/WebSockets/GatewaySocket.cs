using Discord.API.Client;
using Discord.API.Client.GatewaySocket;
using Discord.API.Client.Rest;
using Discord.Logging;
using Discord.Net.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public class GatewaySocket : WebSocket
    {
        private RestClient _rest;
        private uint _lastSequence;
        private int _reconnects;

        //public string Token { get; private set; }
        public string SessionId { get; private set; }

        public event EventHandler<WebSocketEventEventArgs> ReceivedDispatch = delegate { };
        private void OnReceivedDispatch(string type, JToken payload)
            => ReceivedDispatch(this, new WebSocketEventEventArgs(type, payload));

        public GatewaySocket(DiscordConfig config, JsonSerializer serializer, Logger logger)
			: base(config, serializer, logger)
		{
			Disconnected += async (s, e) =>
			{
				if (e.WasUnexpected)
					await Reconnect().ConfigureAwait(false);
			};
		}
        
        public async Task Connect(RestClient rest, CancellationToken parentCancelToken)
        {
            _rest = rest;
            //Token = rest.Token;

            var gatewayResponse = await rest.Send(new GatewayRequest()).ConfigureAwait(false);
            string url = $"{gatewayResponse.Url}/?encoding=json&v=4";
            Logger.Verbose($"Login successful, gateway: {url}");

            Host = url;
            await BeginConnect(parentCancelToken).ConfigureAwait(false);
            if (SessionId == null)
                SendIdentify(_rest.Token);
            else
                SendResume();
        }
		private async Task Reconnect()
		{
			try
			{
				var cancelToken = _parentCancelToken;
                if (_reconnects++ == 0)
				    await Task.Delay(_config.ReconnectDelay, cancelToken).ConfigureAwait(false);
                else
                    await Task.Delay(_config.FailedReconnectDelay, cancelToken).ConfigureAwait(false);

                while (!cancelToken.IsCancellationRequested)
				{
					try
                    {
                        await Connect(_rest, _parentCancelToken).ConfigureAwait(false);
						break;
					}
					catch (OperationCanceledException) { throw; }
					catch (Exception ex)
					{
						Logger.Error("Reconnect failed", ex);
						//Net is down? We can keep trying to reconnect until the user runs Disconnect()
						await Task.Delay(_config.FailedReconnectDelay, cancelToken).ConfigureAwait(false);
					}
				}
			}
			catch (OperationCanceledException) { }
		}
        public async Task Disconnect()
        {
            await _taskManager.Stop(true).ConfigureAwait(false);
            //Token = null;
            SessionId = null;
        }

		protected override async Task Run()
        {
            List<Task> tasks = new List<Task>();
            tasks.AddRange(_engine.GetTasks(CancelToken));
            tasks.Add(HeartbeatAsync(CancelToken));
            await _taskManager.Start(tasks, _cancelSource).ConfigureAwait(false);
        }
        protected override Task Cleanup()
        {
            var ex = _taskManager.Exception;
            if (ex == null || (ex as WebSocketException)?.Code != 1012) //if (ex == null || (ex as WebSocketException)?.Code != 1012)
                SessionId = null; //Reset session unless close code 1012
            return base.Cleanup();
        }

        protected override async Task ProcessMessage(string json)
		{
            base.ProcessMessage(json).GetAwaiter().GetResult(); //This is just a CompletedTask, and we need to avoid asyncs in here

            WebSocketMessage msg;
            using (var reader = new JsonTextReader(new StringReader(json)))
                msg = _serializer.Deserialize(reader, typeof(WebSocketMessage)) as WebSocketMessage;
            
			if (msg.Sequence.HasValue)
				_lastSequence = msg.Sequence.Value;

			var opCode = (OpCodes?)msg.Operation;
            switch (opCode)
			{
				case OpCodes.Dispatch:
					{
                        if (msg.Type == "READY")
                            SessionId = (msg.Payload as JToken).Value<string>("session_id");

                        OnReceivedDispatch(msg.Type, msg.Payload as JToken);

                        if (msg.Type == "READY" || msg.Type == "RESUMED")
                        {
                            _heartbeatInterval = (msg.Payload as JToken).Value<int>("heartbeat_interval");
                            _reconnects = 0;
                            await EndConnect().ConfigureAwait(false); //Complete the connect
                        }
					}
					break;
				case OpCodes.Reconnect:
					{
						var payload = (msg.Payload as JToken).ToObject<ReconnectEvent>(_serializer);
						await Reconnect().ConfigureAwait(false);
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
                Token = token,
                Properties = props, 
                LargeThreshold = _config.LargeThreshold,
                UseCompression = true
            };
			QueueMessage(msg);
		}

        public void SendResume()
            => QueueMessage(new ResumeCommand { SessionId = SessionId, Sequence = _lastSequence });
		public override void SendHeartbeat() 
            => QueueMessage(new HeartbeatCommand());
		public void SendUpdateStatus(long? idleSince, string gameName) 
            => QueueMessage(new UpdateStatusCommand
            {
                IdleSince = idleSince,
                Game = gameName != null ? new UpdateStatusCommand.GameInfo { Name = gameName } : null
            });
		public void SendUpdateVoice(ulong? serverId, ulong? channelId, bool isSelfMuted, bool isSelfDeafened)
            => QueueMessage(new UpdateVoiceCommand { GuildId = serverId, ChannelId = channelId, IsSelfMuted = isSelfMuted, IsSelfDeafened = isSelfDeafened });
		public void SendRequestMembers(IEnumerable<ulong> serverId, string query, int limit)
            => QueueMessage(new RequestMembersCommand { GuildId = serverId.ToArray(), Query = query, Limit = limit });

        //Cancel if either DiscordClient.Disconnect is called, data socket errors or timeout is reached
        public override void WaitForConnection(CancellationToken cancelToken)
            => base.WaitForConnection(CancellationTokenSource.CreateLinkedTokenSource(cancelToken, CancelToken).Token);
    }
}
