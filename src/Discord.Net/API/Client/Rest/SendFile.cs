using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SendFileRequest : IRestFileRequest<Message>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages";
        object IRestRequest.Payload => null;
        string IRestFileRequest.Filename => Filename;
        Stream IRestFileRequest.Stream => Stream;

        public ulong ChannelId { get; set; }

        public string Filename { get; set; }
        public Stream Stream { get; set; }

        public SendFileRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}
