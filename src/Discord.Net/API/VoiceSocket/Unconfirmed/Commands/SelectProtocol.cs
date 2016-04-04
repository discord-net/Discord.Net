using Newtonsoft.Json;

namespace Discord.API.VoiceSocket
{
    public class SelectProtocolCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCode.SelectProtocol;
        object IWebSocketMessage.Payload => this;

        public class ProtocolData
        {
            [JsonProperty("address")]
            public string Address { get; set; }
            [JsonProperty("port")]
            public int Port { get; set; }
            [JsonProperty("mode")]
            public string Mode { get; set; }
        }
        [JsonProperty("protocol")]
        public string Protocol { get; set; } = "udp";
        [JsonProperty("data")]
        private ProtocolData Data { get; } = new ProtocolData();

        public string ExternalAddress { get { return Data.Address; } set { Data.Address = value; } }
        public int ExternalPort { get { return Data.Port; } set { Data.Port = value; } }
        public string EncryptionMode { get { return Data.Mode; } set { Data.Mode = value; } }
    }
}
