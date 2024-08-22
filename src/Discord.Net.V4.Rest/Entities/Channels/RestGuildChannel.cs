using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Extensions;
using Discord.Rest;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestGuildChannelActor :
    RestChannelActor,
    IGuildChannelActor,
    IRestActor<ulong, RestGuildChannel, GuildChannelIdentity, IGuildChannelModel>
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth] public GuildChannelInviteLink.Enumerable.Indexable Invites { get; }

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
            entityFactory: RestGuildChannelInvite.Construct,
            new RestGuildChannelInvite.Context(guild, channel),
            IGuildChannelInvite.FetchManyRoute(this)
        );
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal virtual RestGuildChannel CreateEntity(IGuildChannelModel model)
        => RestGuildChannel.Construct(Client, Guild.Identity, model);

    [SourceOfTruth]
    internal RestGuildChannelInvite CreateEntity(IInviteModel model)
        => RestGuildChannelInvite.Construct(Client, new(Guild.Identity, Identity), model);
}

public partial class RestGuildChannel :
    RestChannel,
    IGuildChannel,
    IRestConstructable<RestGuildChannel, RestGuildChannelActor, IGuildChannelModel>
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
        IGuildChannelModel model,
        RestGuildChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor;
        
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
