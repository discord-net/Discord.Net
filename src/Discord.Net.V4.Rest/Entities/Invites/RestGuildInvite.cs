using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestGuildInviteActor :
    RestInviteActor,
    IGuildInviteActor,
    IRestActor<string, RestGuildInvite, GuildInviteIdentity, IInviteModel>
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth] internal override GuildInviteIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(invite))]
    public RestGuildInviteActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildInviteIdentity invite
    ) : base(client, invite)
    {
        Identity = invite | this;

        Guild = guild.Actor ?? client.Guilds[guild.Id];
    }

    [SourceOfTruth]
    internal override RestGuildInvite CreateEntity(IInviteModel model)
        => RestGuildInvite.Construct(Client, this, model);
}

[ExtendInterfaceDefaults]
public partial class RestGuildInvite :
    RestInvite,
    IGuildInvite,
    IRestConstructable<RestGuildInvite, RestGuildInviteActor, IInviteModel>
{
    [SourceOfTruth] public RestGuildScheduledEventActor? GuildScheduledEvent { get; private set; }

    [ProxyInterface] internal override RestGuildInviteActor Actor { get; }

    public RestGuildInvite(
        DiscordRestClient client,
        IInviteModel model,
        RestGuildInviteActor actor
    ) : base(client, model, actor)
    {
        Actor = actor;

        GuildScheduledEvent = model.ScheduledEventId.Map(
            (id, guild) => guild.ScheduledEvents[id],
            actor.Guild
        );
    }

    public static RestGuildInvite Construct(DiscordRestClient client, RestGuildInviteActor actor, IInviteModel model)
        => new(client, model, actor);
}