using Discord.Models;
using Discord.Rest;

namespace Discord.Rest;

public sealed partial class RestGuildScheduledEventUserActor(
    DiscordRestClient client,
    GuildIdentity guild,
    GuildScheduledEventIdentity scheduledEvent,
    GuildScheduledEventUserIdentity eventUser,
    UserIdentity? user = null,
    MemberIdentity? member = null
) :
    RestActor<ulong, RestGuildScheduledEventUser, GuildScheduledEventUserIdentity>(client, eventUser),
    IGuildScheduledEventUserActor
{
    [SourceOfTruth]
    public RestUserActor User { get; } = user?.Actor ?? new(client, user ?? UserIdentity.Of(eventUser.Id));

    [SourceOfTruth]
    public RestMemberActor Member { get; } =
        member?.Actor ?? new(client, guild, member ?? MemberIdentity.Of(eventUser.Id));

    [SourceOfTruth]
    public RestGuildScheduledEventActor GuildScheduledEvent { get; } =
        scheduledEvent.Actor ?? new(client, guild, scheduledEvent);

    [SourceOfTruth] public RestGuildActor Guild { get; } = guild.Actor ?? new(client, guild);
}

public sealed partial class RestGuildScheduledEventUser :
    RestEntity<ulong>,
    IGuildScheduledEventUser
{
    [ProxyInterface(typeof(IGuildScheduledEventUserActor))]
    internal RestGuildScheduledEventUserActor Actor { get; }

    internal IGuildScheduledEventUserModel Model { get; }

    internal RestGuildScheduledEventUser(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildScheduledEventIdentity scheduledEvent,
        IGuildScheduledEventUserModel model
    ) : base(client, model.Id)
    {
        Actor = new(
            client,
            guild,
            scheduledEvent,
            GuildScheduledEventUserIdentity.Of(this),
            UserIdentity.FromReferenced(model, model.Id, model => RestUser.Construct(client, model)),
            MemberIdentity.FromReferenced(model, model.Id, model => RestMember.Construct(client, guild, model))
        );

        Model = model;
    }

    public IGuildScheduledEventUserModel GetModel() => Model;
}
