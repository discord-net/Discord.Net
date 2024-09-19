using Discord.Models;
using Discord.Rest;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestGuildScheduledEventUserActor :
    RestActor<RestGuildScheduledEventUserActor, ulong, RestGuildScheduledEventUser, IGuildScheduledEventUserModel>,
    IGuildScheduledEventUserActor
{
    [SourceOfTruth] public RestUserActor User { get; }

    [SourceOfTruth] public RestMemberActor Member { get; }

    [SourceOfTruth] public RestGuildScheduledEventActor GuildScheduledEvent { get; }

    [SourceOfTruth] public RestGuildActor Guild { get; }

    internal override GuildScheduledEventUserIdentity Identity { get; }

    public RestGuildScheduledEventUserActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildScheduledEventIdentity scheduledEvent,
        GuildScheduledEventUserIdentity eventUser,
        UserIdentity? user = null,
        MemberIdentity? member = null
    ) : base(client, eventUser)
    {
        Identity = eventUser | this;

        User = user?.Actor ?? new(client, user ?? UserIdentity.Of(eventUser.Id));
        Guild = guild.Actor ?? new(client, guild);
        Member = member?.Actor ?? new(client, Guild.Identity, member ?? MemberIdentity.Of(eventUser.Id), user | User);
        GuildScheduledEvent = scheduledEvent.Actor ?? new(client, Guild.Identity, scheduledEvent);
    }

    [SourceOfTruth]
    internal override RestGuildScheduledEventUser CreateEntity(IGuildScheduledEventUserModel model)
        => RestGuildScheduledEventUser.Construct(Client, this, model);
}

public sealed partial class RestGuildScheduledEventUser :
    RestEntity<ulong>,
    IGuildScheduledEventUser,
    IRestConstructable<
        RestGuildScheduledEventUser,
        RestGuildScheduledEventUserActor,
        IGuildScheduledEventUserModel
    >
{
    [ProxyInterface(typeof(IGuildScheduledEventUserActor))]
    internal RestGuildScheduledEventUserActor Actor { get; }

    internal IGuildScheduledEventUserModel Model { get; }

    internal RestGuildScheduledEventUser(
        DiscordRestClient client,
        IGuildScheduledEventUserModel model,
        RestGuildScheduledEventUserActor actor
    ) : base(client, model.Id)
    {
        Actor = actor;
        Model = model;
    }

    public static RestGuildScheduledEventUser Construct(
        DiscordRestClient client,
        RestGuildScheduledEventUserActor actor,
        IGuildScheduledEventUserModel model
    ) => new(client, model, actor);

    public IGuildScheduledEventUserModel GetModel() => Model;
}