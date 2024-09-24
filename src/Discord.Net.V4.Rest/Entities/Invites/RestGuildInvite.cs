using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestGuildInviteActor :
    RestInviteActor,
    IGuildInviteActor,
    IRestActor<RestGuildInviteActor, string, RestGuildInvite, IInviteModel>
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
    [SourceOfTruth]
    public RestGuildScheduledEventActor? GuildScheduledEvent
        => Computed(nameof(GuildScheduledEvent), model => 
            model.ScheduledEventId.HasValue 
                ? Actor.Guild.ScheduledEvents[model.ScheduledEventId.Value]
                : null
        );

    [ProxyInterface] internal override RestGuildInviteActor Actor { get; }

    public RestGuildInvite(
        DiscordRestClient client,
        IInviteModel model,
        RestGuildInviteActor actor
    ) : base(client, model, actor)
    {
        Actor = actor;
    }

    public static RestGuildInvite Construct(DiscordRestClient client, RestGuildInviteActor actor, IInviteModel model)
        => new(client, model, actor);
}