using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Discord.Rest.Channels;

[ExtendInterfaceDefaults(
    typeof(IGuildChannelActor),
    typeof(IDeletable<ulong, IGuildChannelActor>)
)]
public partial class RestGuildChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    GuildChannelIdentity channel
) :
    RestChannelActor(client, channel),
    IGuildChannelActor,
    IActor<ulong, RestGuildChannel>
{
    [SourceOfTruth]
    public RestLoadableGuildActor Guild { get; } = new(client, guild);

    public IEnumerableIndexableActor<IInviteActor, string, IInvite> Invites => throw new NotImplementedException();

    [SourceOfTruth]
    internal virtual RestGuildChannel CreateEntity(IGuildChannelModel model)
        => RestGuildChannel.Construct(Client, model, Guild.Identity);
}

public partial class RestGuildChannel :
    RestChannel,
    IGuildChannel,
    IContextConstructable<RestGuildChannel, IGuildChannelModel, GuildIdentity, DiscordRestClient>
{
    public int Position => Model.Position;

    public ChannelFlags Flags => (ChannelFlags?)Model.Flags ?? ChannelFlags.None;

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
        if (!_model.Permissions.SequenceEqual(model.Permissions))
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
