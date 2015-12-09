//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.API.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Discord.API
{
    public enum GatewayOpCodes : byte
    {
        /// <summary> Client <-- Server - Used to send most events. </summary>
        Dispatch = 0,
        /// <summary> Client <-> Server - Used to keep the connection alive and measure latency. </summary>
        Heartbeat = 1,
        /// <summary> Client --> Server - Used to associate a connection with a token and specify configuration. </summary>
        Identify = 2,
        /// <summary> Client --> Server - Used to update client's status and current game id. </summary>
        StatusUpdate = 3,
        /// <summary> Client --> Server - Used to join a particular voice channel. </summary>
        VoiceStateUpdate = 4,
        /// <summary> Client --> Server - Used to ensure the server's voice server is alive. Only send this if voice connection fails or suddenly drops. </summary>
        VoiceServerPing = 5,
        /// <summary> Client --> Server - Used to resume a connection after a redirect occurs. </summary>
        Resume = 6,
        /// <summary> Client <-- Server - Used to notify a client that they must reconnect to another gateway. </summary>
        Redirect = 7,
        /// <summary> Client --> Server - Used to request all members that were withheld by large_threshold </summary>
        RequestGuildMembers = 8
    }

    //Common
    public class WebSocketMessage
    {
        public WebSocketMessage() { }
        public WebSocketMessage(int op) { Operation = op; }

        [JsonProperty("op")]
        public int Operation;
        [JsonProperty("d")]
        public object Payload;
        [JsonProperty("t", NullValueHandling = NullValueHandling.Ignore)]
        public string Type;
        [JsonProperty("s", NullValueHandling = NullValueHandling.Ignore)]
        public int? Sequence;
    }
    public abstract class WebSocketMessage<T> : WebSocketMessage
        where T : new()
    {
        public WebSocketMessage() { Payload = new T(); }
        public WebSocketMessage(int op) : base(op) { Payload = new T(); }
        public WebSocketMessage(int op, T payload) : base(op) { Payload = payload; }

        [JsonIgnore]
        public new T Payload
        {
            get
            {
                if (base.Payload is JToken)
                    base.Payload = (base.Payload as JToken).ToObject<T>();
                return (T)base.Payload;
            }
            set { base.Payload = value; }
        }
    }

    //Commands
    internal sealed class HeartbeatCommand : WebSocketMessage<long>
    {
        public HeartbeatCommand() : base((int)GatewayOpCodes.Heartbeat, EpochTime.GetMilliseconds()) { }
    }
    internal sealed class IdentifyCommand : WebSocketMessage<IdentifyCommand.Data>
    {
        public IdentifyCommand() : base((int)GatewayOpCodes.Identify) { }
        public class Data
        {
            [JsonProperty("token")]
            public string Token;
            [JsonProperty("v")]
            public int Version = 3;
            [JsonProperty("properties")]
            public Dictionary<string, string> Properties = new Dictionary<string, string>();
            [JsonProperty("large_threshold", NullValueHandling = NullValueHandling.Ignore)]
            public int? LargeThreshold;
            [JsonProperty("compress", NullValueHandling = NullValueHandling.Ignore)]
            public bool? Compress;
        }
    }

    internal sealed class StatusUpdateCommand : WebSocketMessage<StatusUpdateCommand.Data>
    {
        public StatusUpdateCommand() : base((int)GatewayOpCodes.StatusUpdate) { }
        public class Data
        {
            [JsonProperty("idle_since")]
            public long? IdleSince;
            [JsonProperty("game_id")]
            public int? GameId;
        }
    }

    internal sealed class JoinVoiceCommand : WebSocketMessage<JoinVoiceCommand.Data>
    {
        public JoinVoiceCommand() : base((int)GatewayOpCodes.VoiceStateUpdate) { }
        public class Data
        {
            [JsonProperty("guild_id")]
            [JsonConverter(typeof(LongStringConverter))]
            public long ServerId;
            [JsonProperty("channel_id")]
            [JsonConverter(typeof(LongStringConverter))]
            public long ChannelId;
            [JsonProperty("self_mute")]
            public string SelfMute;
            [JsonProperty("self_deaf")]
            public string SelfDeaf;
        }
    }

    internal sealed class ResumeCommand : WebSocketMessage<ResumeCommand.Data>
    {
        public ResumeCommand() : base((int)GatewayOpCodes.Resume) { }
        public class Data
        {
            [JsonProperty("session_id")]
            public string SessionId;
            [JsonProperty("seq")]
            public int Sequence;
        }
    }

    //Events
    internal sealed class ReadyEvent
    {
        public sealed class ReadStateInfo
        {
            [JsonProperty("id")]
            public string ChannelId;
            [JsonProperty("mention_count")]
            public int MentionCount;
            [JsonProperty("last_message_id")]
            public string LastMessageId;
        }

        [JsonProperty("v")]
        public int Version;
        [JsonProperty("user")]
        public UserInfo User;
        [JsonProperty("session_id")]
        public string SessionId;
        [JsonProperty("read_state")]
        public ReadStateInfo[] ReadState;
        [JsonProperty("guilds")]
        public ExtendedGuildInfo[] Guilds;
        [JsonProperty("private_channels")]
        public ChannelInfo[] PrivateChannels;
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval;
    }

    internal sealed class RedirectEvent
    {
        [JsonProperty("url")]
        public string Url;
    }
    internal sealed class ResumeEvent
    {
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval;
    }
}
