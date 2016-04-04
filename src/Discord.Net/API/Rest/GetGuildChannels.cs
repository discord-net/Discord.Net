using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetGuildChannelsRequest : IRestRequest<Channel[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guild/{GuildId}/channels";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }

        public GetGuildChannelsRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
