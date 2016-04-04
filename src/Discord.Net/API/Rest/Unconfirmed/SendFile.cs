using Discord.Net.Rest;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace Discord.API.Rest
{
    public class SendFileRequest : IRestFileRequest<Message>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages";
        object IRestRequest.Payload => null;

        string IRestFileRequest.Filename => Filename;
        Stream IRestFileRequest.Stream => Stream;
        IReadOnlyList<RestParameter> IRestFileRequest.MultipartParameters => ImmutableArray.Create(
            new RestParameter("content", Content),
            new RestParameter("nonce", Nonce),
            new RestParameter("tts", IsTTS)
        );

        public ulong ChannelId { get; }

        public string Filename { get; set; }
        public Stream Stream { get; set; }
        public string Content { get; set; }
        public string Nonce { get; set; }
        public bool IsTTS { get; set; }

        public SendFileRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}
