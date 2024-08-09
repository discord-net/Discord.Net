using Discord.Models;
using Discord.Rest;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestInviteActor :
    RestActor<string, RestInvite, InviteIdentity>,
    IInviteActor
{
    internal override InviteIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(invite))]
    public RestInviteActor(
        DiscordRestClient client,
        InviteIdentity invite
    ) : base(client, invite)
    {
        Identity = invite | this;
    }

    internal static RestInviteActor Create(
        DiscordRestClient client,
        InviteIdentity invite,
        GuildIdentity? guild = null,
        GuildChannelIdentity? channel = null)
    {
        if (channel is not null && guild is not null)
            return new RestGuildChannelInviteActor(client, guild, channel, invite);

        if (guild is not null)
            return new RestGuildInviteActor(client, guild, invite);

        return new RestInviteActor(client, invite);
    }

    [SourceOfTruth]
    internal virtual RestInvite CreateEntity(IInviteModel model)
        => RestInvite.Construct(Client, model);
}

[ExtendInterfaceDefaults]
public partial class RestGuildInviteActor :
    RestInviteActor,
    IGuildInviteActor
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    internal override InviteIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(invite))]
    public RestGuildInviteActor(
        DiscordRestClient client,
        GuildIdentity guild,
        InviteIdentity invite
    ) : base(client, invite)
    {
        Identity = invite | this;

        Guild = guild.Actor ?? new(client, guild);
    }

    internal override RestInvite CreateEntity(IInviteModel model)
        => RestInvite.Construct(Client, new(Guild.Identity), model);
}

[ExtendInterfaceDefaults]
public sealed partial class RestGuildChannelInviteActor :
    RestGuildInviteActor,
    IGuildChannelInviteActor
{
    [SourceOfTruth]
    public RestGuildChannelActor Channel { get; }

    internal override InviteIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(invite))]
    public RestGuildChannelInviteActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildChannelIdentity channel,
        InviteIdentity invite
    ) : base(client, guild, invite)
    {
        Identity = invite | this;

        Channel = channel.Actor ?? new(client, guild, channel);
    }

    internal override RestInvite CreateEntity(IInviteModel model)
        => RestInvite.Construct(Client, new(Guild.Identity, Channel.Identity), model);
}

public sealed partial class RestInvite :
    RestEntity<string>,
    IInvite,
    IConstructable<RestInvite, IInviteModel, DiscordRestClient>,
    IContextConstructable<RestInvite, IInviteModel, RestInvite.Context, DiscordRestClient>
{
    public readonly record struct Context(GuildIdentity? Guild = null, GuildChannelIdentity? Channel = null);

    public InviteType Type => (InviteType)Model.Type;

    [SourceOfTruth] public RestGuildActor? Guild { get; }

    [SourceOfTruth] public RestGuildChannelActor? Channel { get; }

    [SourceOfTruth] public RestUserActor? Inviter { get; private set; }

    public InviteTargetType? TargetType => (InviteTargetType?)Model.TargetType;

    [SourceOfTruth] public RestUserActor? TargetUser { get; private set; }

    public int? ApproximatePresenceCount => Model.ApproximatePresenceCount;

    public int? ApproximateMemberCount => Model.ApproximateMemberCount;

    public DateTimeOffset? ExpiresAt => Model.ExpiresAt;

    [SourceOfTruth] public RestGuildScheduledEventActor? GuildScheduledEvent { get; private set; }

    [ProxyInterface(typeof(IInviteActor))] internal RestInviteActor Actor { get; }

    internal IInviteModel Model { get; private set; }

    public RestInvite(
        DiscordRestClient client,
        IInviteModel model,
        RestInviteActor? actor = null,
        GuildIdentity? guild = null,
        GuildChannelIdentity? channel = null
    ) : base(client, model.Id)
    {
        Model = model;

        if (guild is null && model.GuildId.HasValue)
            guild = GuildIdentity.Of(model.GuildId.Value);

        if (channel is null && model.ChannelId.HasValue)
            channel = GuildChannelIdentity.Of(model.ChannelId.Value);

        Actor = actor ?? RestInviteActor.Create(client, InviteIdentity.Of(this), guild, channel);

        Guild = Actor is RestGuildInviteActor guildActor ? guildActor.Guild : null;
        Channel = Actor is RestGuildChannelInviteActor guildChannelActor ? guildChannelActor.Channel : null;

        Inviter = model.InviterId.Map(
            static (id, client) => RestUserActor.Factory(client, UserIdentity.Of(id)),
            client
        );

        TargetUser = model.TargetUserId.Map(
            static (id, client) => RestUserActor.Factory(client, UserIdentity.Of(id)),
            client
        );
    }

    public static RestInvite Construct(DiscordRestClient client, IInviteModel model)
        => Construct(client, default, model);

    public static RestInvite Construct(DiscordRestClient client, Context context, IInviteModel model)
        => new(client, model, guild: context.Guild, channel: context.Channel);

    public ValueTask UpdateAsync(IInviteModel model, CancellationToken token = default)
    {
        Inviter = Inviter.UpdateFrom(
            model.InviterId,
            RestUserActor.Factory,
            Client
        );

        TargetUser = TargetUser.UpdateFrom(
            model.TargetUserId,
            RestUserActor.Factory,
            Client
        );

        Model = model;

        return ValueTask.CompletedTask;
    }

    public IInviteModel GetModel() => Model;
}
