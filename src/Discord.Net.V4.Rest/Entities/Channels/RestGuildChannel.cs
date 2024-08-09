using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Extensions;
using Discord.Rest;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Discord.Rest;

using EnumerableInvitesActor = RestEnumerableIndexableActor<
    RestGuildChannelInviteActor,
    string,
    RestInvite,
    IInvite,
    IEnumerable<IInviteModel>
>;

[ExtendInterfaceDefaults]
public partial class RestGuildChannelActor :
    RestChannelActor,
    IGuildChannelActor,
    IRestActor<ulong, RestGuildChannel, GuildChannelIdentity>
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth] public EnumerableInvitesActor Invites { get; }

    [SourceOfTruth]
    internal override GuildChannelIdentity Identity { get; }

    [TypeFactory]
    public RestGuildChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildChannelIdentity channel
    ) : base(client, channel)
    {
        channel = Identity = channel | this;

        Guild = guild.Actor ?? new RestGuildActor(client, guild);

        Invites = RestActors.Fetchable(
            Template.T<RestGuildChannelInviteActor>(),
            Client,
            RestGuildChannelInviteActor.Factory,
            guild,
            channel,
            entityFactory: RestInvite.Construct,
            new RestInvite.Context(guild, channel),
            IInvite.GetChannelInvitesRoute(this)
        );
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal virtual RestGuildChannel CreateEntity(IGuildChannelModel model)
        => RestGuildChannel.Construct(Client, Guild.Identity, model);

    [SourceOfTruth]
    internal RestInvite CreateEntity(IInviteModel model)
        => RestInvite.Construct(Client, new(Guild.Identity, Identity), model);
}

public partial class RestGuildChannel :
    RestChannel,
    IGuildChannel,
    IContextConstructable<RestGuildChannel, IGuildChannelModel, GuildIdentity, DiscordRestClient>
{
    public string Name => Model.Name;

    public int Position => Model.Position;

    public ChannelFlags Flags => (ChannelFlags?)Model.Flags ?? ChannelFlags.None;

    public IReadOnlyCollection<Overwrite> PermissionOverwrites { get; private set; }

    [ProxyInterface(
        typeof(IGuildChannelActor)
    )]
    internal override RestGuildChannelActor Actor { get; }

    internal override IGuildChannelModel Model => _model;

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

    public static RestGuildChannel Construct(DiscordRestClient client,
        GuildIdentity guild,
        IGuildChannelModel model)
    {
        return model switch
        {
            IGuildForumChannelModel guildForumChannelModel => RestForumChannel.Construct(client,
                guild, guildForumChannelModel),
            IGuildMediaChannelModel guildMediaChannelModel => RestMediaChannel.Construct(client,
                guild, guildMediaChannelModel),
            IGuildNewsChannelModel guildNewsChannelModel => RestNewsChannel.Construct(client,
                guild, guildNewsChannelModel),
            IGuildVoiceChannelModel guildVoiceChannelModel => RestVoiceChannel.Construct(client,
                guild, guildVoiceChannelModel),
            IThreadChannelModel threadChannelModel => RestThreadChannel.Construct(client, new RestThreadChannel.Context(
                guild
            ), threadChannelModel),
            IGuildTextChannelModel guildTextChannelModel => RestTextChannel.Construct(client,
                guild, guildTextChannelModel),
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
