using Discord.Models;

namespace Discord.Rest.Channels;

public partial class RestChannel(DiscordRestClient client, IChannelModel model) :
    RestEntity<ulong>(client, model.Id),
    IChannel
{
    internal virtual IChannelModel Model { get; } = model;

    public ChannelType Type => (ChannelType)Model.Type;
}
