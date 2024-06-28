using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;
using PropertyChanged;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Discord.Rest.Channels;

public sealed partial class RestLoadableGuildChannel(
    DiscordRestClient client,
    IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
    IIdentifiableEntityOrModel<ulong, RestGuildChannel, IChannelModel> channel) :
    RestGuildChannelActor(client, guild, channel.Id),
    ILoadableGuildChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuildChannel>))]
    public RestLoadable<ulong, RestGuildChannel, IGuildChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            (_, model) =>
            {
                if (model is null)
                    return null;

                if (model is not GuildChannelBase guildChannelModel)
                    throw new DiscordException($"Channel type is not a guild channel ({(ChannelType)model.Type})");

                return RestGuildChannel.Construct(client, guildChannelModel, guild);
            }
        );
}

[ExtendInterfaceDefaults(
    typeof(IGuildChannelActor),
    typeof(IDeletable<ulong, IGuildChannelActor>),
    typeof(IModifiable<ulong, IGuildChannelActor, ModifyGuildChannelProperties, ModifyGuildChannelParams>)
)]
public partial class RestGuildChannelActor(
    DiscordRestClient client,
    IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
    IIdentifiableEntityOrModel<ulong, RestGuildChannel, IGuildChannelModel> channel) :
    RestChannelActor(client, channel),
    IGuildChannelActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guild);

    public IEnumerableIndexableActor<ILoadableInviteActor<IInvite>, string, IInvite> Invites => throw new NotImplementedException();

    ILoadableGuildActor IGuildRelationship.Guild => Guild;
}

public partial class RestGuildChannel(
    DiscordRestClient client,
    IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
    IGuildChannelModel model,
    RestGuildChannelActor? actor = null
):
    RestChannel(client, model),
    IGuildChannel,
    IContextConstructable<RestGuildChannel, IGuildChannelModel, RestGuildIdentifiable, DiscordRestClient>,
    INotifyPropertyChanged
{
    [OnChangedMethod(nameof(OnModelChanged))]
    internal new IGuildChannelModel Model { get; set; } = model;

    [ProxyInterface(typeof(IGuildChannelActor), typeof(IGuildRelationship))]
    internal override RestGuildChannelActor ChannelActor { get; } = actor ?? new RestGuildChannelActor(client, guild, model.Id);

    public static RestGuildChannel Construct(
        DiscordRestClient client,
        IGuildChannelModel model,
        IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild)
    {
        return model switch
        {
            IGuildForumChannelModel guildForumChannelModel => RestForumChannel.Construct(client, guildForumChannelModel,
                guild),
            IGuildMediaChannelModel guildMediaChannelModel => RestMediaChannel.Construct(client, guildMediaChannelModel,
                guild),
            IGuildNewsChannelModel guildNewsChannelModel => RestNewsChannel.Construct(client, guildNewsChannelModel,
                guild),
            IGuildVoiceChannelModel guildVoiceChannelModel => RestVoiceChannel.Construct(client, guildVoiceChannelModel,
                guild),
            IThreadChannelModel threadChannelModel => RestThreadChannel.Construct(client, threadChannelModel, guild),
            IGuildTextChannelModel guildTextChannelModel => RestTextChannel.Construct(client, guildTextChannelModel,
                guild),
            _ => new(client, guild, model)
        };
    }

    private void OnModelChanged()
    {
        PermissionOverwrites = Model.Permissions
            .Select(x => Overwrite.Construct(Client, x))
            .ToImmutableArray();
    }

    public int Position => Model.Position;

    public ChannelFlags Flags => (ChannelFlags?)Model.Flags ?? ChannelFlags.None;

    public IReadOnlyCollection<Overwrite> PermissionOverwrites { get; private set; } =
        model.Permissions.Select(x => Overwrite.Construct(client, x)).ToImmutableArray();

}
