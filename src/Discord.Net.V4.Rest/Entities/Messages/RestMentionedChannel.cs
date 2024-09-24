using Discord.Models;

namespace Discord.Rest;

public sealed partial class RestMentionedChannel : 
    IMentionedChannel,
    IModelConstructable<RestMentionedChannel, IMentionedChannelModel, DiscordRestClient>
{
    [SourceOfTruth]
    public RestGuildChannelActor Channel { get; }

    [SourceOfTruth]
    public RestGuildActor Guild { get; }

    public string Name { get; }

    public ChannelType Type { get; }

    internal RestMentionedChannel(DiscordRestClient client, IMentionedChannelModel model)
    {
        Type = (ChannelType)model.Type;
        Guild = client.Guilds[model.GuildId];
        Channel = Guild.Channels.OfType(Type)[model.Id];
        Name = model.Name;
    }

    public static RestMentionedChannel Construct(DiscordRestClient client, IMentionedChannelModel model)
        => new(client, model);
}