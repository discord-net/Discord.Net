using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestGuildInviteActor :
    RestInviteActor,
    IGuildInviteActor,
    IRestActor<string, RestGuildInvite, GuildInviteIdentity>
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth]
    internal override GuildInviteIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(invite))]
    public RestGuildInviteActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildInviteIdentity invite
    ) : base(client, invite)
    {
        Identity = invite | this;

        Guild = guild.Actor ?? new(client, guild);
    }

    internal override RestInvite CreateEntity(IInviteModel model)
        => RestInvite.Construct(Client, new(Guild.Identity), model);
}

[ExtendInterfaceDefaults]
public partial class RestGuildInvite :
    RestInvite,
    IGuildInvite,
    IRestConstructable<RestGuildInvite, RestGuildInviteActor, IInviteModel>
{
    public new readonly record struct Context(
        GuildIdentity Guild,
        GuildChannelIdentity? Channel = null,
        GuildScheduledEventIdentity? ScheduledEvent = null
    );

    [SourceOfTruth] public RestGuildScheduledEventActor? GuildScheduledEvent { get; private set; }

    [ProxyInterface] internal override RestGuildInviteActor Actor { get; }

    public RestGuildInvite(
        DiscordRestClient client,
        GuildIdentity guild,
        IInviteModel model,
        RestGuildInviteActor? actor = null,
        GuildScheduledEventIdentity? scheduledEvent = null
    ) : base(client, model, actor)
    {
        actor = Actor = actor ?? new(client, guild, GuildInviteIdentity.Of(this));

        GuildScheduledEvent = model.ScheduledEventId.Map(
            (id, guild) => guild.ScheduledEvents[id],
            actor.Guild
        );
    }

    public static RestGuildInvite Construct(DiscordRestClient client, Context context, IInviteModel model)
    {
        if(context.Channel is not null || model.ChannelId.HasValue)
            return RestGuildChannelInvite.Construct(
                client,
                new RestGuildChannelInvite.Context(
                    context.Guild,
                    context.Channel | model.ChannelId!.Value,
                    context.ScheduledEvent
                ),
                model
            );

        return new RestGuildInvite(client, context.Guild, model, scheduledEvent: context.ScheduledEvent);
    }
}
