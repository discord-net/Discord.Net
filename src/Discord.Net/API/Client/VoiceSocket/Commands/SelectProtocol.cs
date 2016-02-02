using Newtonsoft.Json;

namespace Discord.API.Client.VoiceSocket
{
    public class SelectProtocolCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCodes.SelectProtocol;
        object IWebSocketMessage.Payload => this;
        bool IWebSocketMessage.IsPrivate => false;

        public class Data
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
        private Data ProtocolData { get; } = new Data();

        public string ExternalAddress { get { return ProtocolData.Address; } set { ProtocolData.Address = value; } }
        public int ExternalPort { get { return ProtocolData.Port; } set { ProtocolData.Port = value; } }
        public string EncryptionMode { get { return ProtocolData.Mode; } set { ProtocolData.Mode = value; } }
    }
}
