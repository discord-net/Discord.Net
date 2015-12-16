using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class SendFileRequest : IRestFileRequest<Message>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;
        string IRestFileRequest.Filename => Filename;
        Stream IRestFileRequest.Stream => Stream;

        public ulong ChannelId { get; }

        public string Filename { get; set; }
        public Stream Stream { get; set; }

        public SendFileRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}
