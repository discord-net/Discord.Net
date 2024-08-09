using Discord.Models;
using Discord.Rest;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestGuildScheduledEventUserActor :
    RestActor<ulong, RestGuildScheduledEventUser, GuildScheduledEventUserIdentity>,
    IGuildScheduledEventUserActor
{
    [SourceOfTruth] public RestUserActor User { get; }

    [SourceOfTruth] public RestMemberActor Member { get; }

    [SourceOfTruth] public RestGuildScheduledEventActor GuildScheduledEvent { get; }

    [SourceOfTruth] public RestGuildActor Guild { get; }

    internal override GuildScheduledEventUserIdentity Identity { get; }

    public RestGuildScheduledEventUserActor(DiscordRestClient client,
        GuildIdentity guild,
        GuildScheduledEventIdentity scheduledEvent,
        GuildScheduledEventUserIdentity eventUser,
        UserIdentity? user = null,
        MemberIdentity? member = null) : base(client, eventUser)
    {
        Identity = eventUser | this;

        User = user?.Actor ?? new(client, user ?? UserIdentity.Of(eventUser.Id));
        Guild = guild.Actor ?? new(client, guild);
        Member = member?.Actor ?? new(client, Guild.Identity, member ?? MemberIdentity.Of(eventUser.Id), user | User);
        GuildScheduledEvent = scheduledEvent.Actor ?? new(client, Guild.Identity, scheduledEvent);
    }
}

public sealed partial class RestGuildScheduledEventUser :
    RestEntity<ulong>,
    IGuildScheduledEventUser,
    IContextConstructable<
        RestGuildScheduledEventUser,
        IGuildScheduledEventUserModel,
        RestGuildScheduledEventUser.Context,
        DiscordRestClient
    >
{
    public readonly record struct Context(GuildIdentity Guild, GuildScheduledEventIdentity Event);

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

    public static RestGuildScheduledEventUser Construct(
        DiscordRestClient client,
        Context context,
        IGuildScheduledEventUserModel model
    ) => new(client, context.Guild, context.Event, model);

    public IGuildScheduledEventUserModel GetModel() => Model;
}
