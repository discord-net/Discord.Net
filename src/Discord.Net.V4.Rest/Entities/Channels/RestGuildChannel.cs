using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Discord.Rest.Channels;

public sealed partial class RestLoadableGuildChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    GuildChannelIdentity channel
):
    RestGuildChannelActor(client, guild, channel),
    ILoadableGuildChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuildChannel>))]
    public RestLoadable<ulong, RestGuildChannel, IGuildChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            (client, _, model) =>
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
    GuildIdentity guild,
    GuildChannelIdentity channel) :
    RestChannelActor(client, channel),
    IGuildChannelActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guild);

    public IEnumerableIndexableActor<ILoadableInviteActor<IInvite>, string, IInvite> Invites => throw new NotImplementedException();

    public IGuildChannel CreateEntity(IGuildChannelModel model)
        => RestGuildChannel.Construct(Client, model, Guild.Identity);

    ILoadableGuildActor IGuildRelationship.Guild => Guild;
}

public partial class RestGuildChannel :
    RestChannel,
    IGuildChannel,
    IContextConstructable<RestGuildChannel, IGuildChannelModel, GuildIdentity, DiscordRestClient>
{
    public int Position => ModelSource.Value.Position;

    public ChannelFlags Flags => (ChannelFlags?)ModelSource.Value.Flags ?? ChannelFlags.None;

    public IReadOnlyCollection<Overwrite> PermissionOverwrites { get; private set; }

    internal override IGuildChannelModel Model => _model;

    [ProxyInterface(
        typeof(IGuildChannelActor),
        typeof(IGuildRelationship),
        typeof(IEntityProvider<IGuildChannel, IGuildChannelModel>)
    )]
    internal override RestGuildChannelActor Actor { get; }

    private IGuildChannelModel _model;

    internal RestGuildChannel(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildChannelModel model,
        RestGuildChannelActor? actor = null
    ) : base(client, model)
    {
        _model = model;

        Actor = actor ?? new RestGuildChannelActor(
            client,
            guild,
            GuildChannelIdentity.Of(this)
        );

        PermissionOverwrites = model.Permissions.Select(x => Overwrite.Construct(client, x)).ToImmutableArray();
    }

    public static RestGuildChannel Construct(
        DiscordRestClient client,
        IGuildChannelModel model,
        GuildIdentity guild)
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

    [CovariantOverride]
    public virtual ValueTask UpdateAsync(IGuildChannelModel model, CancellationToken token = default)
    {
        if (!Model.Permissions.SequenceEqual(model.Permissions))
        {
            PermissionOverwrites = model.Permissions
                .Select(x => Overwrite.Construct(Client, x))
                .ToImmutableArray();
        }

        _model = model;

        return base.UpdateAsync(model, token);
    }
    public override IGuildChannelModel GetModel() => Model;
}
