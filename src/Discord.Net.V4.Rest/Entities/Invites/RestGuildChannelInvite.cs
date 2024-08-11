using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestGuildChannelInviteActor :
    RestGuildInviteActor,
    IGuildChannelInviteActor,
    IRestActor<string, RestGuildChannelInvite, GuildChannelInviteIdentity>
{
    [SourceOfTruth] public RestGuildChannelActor Channel { get; }

    IInvitableChannelTrait IChannelRelationship<IInvitableChannelTrait, IInvitableChannel>.Channel => Channel;

    [SourceOfTruth] internal override GuildChannelInviteIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(invite))]
    public RestGuildChannelInviteActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildChannelIdentity channel,
        GuildChannelInviteIdentity invite
    ) : base(client, guild, invite)
    {
        Identity = invite | this;
        Channel = channel.Actor ?? new(client, guild, channel);
    }

    internal override RestInvite CreateEntity(IInviteModel model)
        => RestInvite.Construct(Client, new(Guild.Identity, Channel.Identity), model);
}

[ExtendInterfaceDefaults]
public sealed partial class RestGuildChannelInvite :
    RestGuildInvite,
    IGuildChannelInvite,
    IContextConstructable<RestGuildChannelInvite, IInviteModel, RestGuildChannelInvite.Context, DiscordRestClient>
{
    public new readonly record struct Context(
        GuildIdentity Guild,
        GuildChannelIdentity Channel,
        GuildScheduledEventIdentity? ScheduledEvent = null
    );

    [ProxyInterface] internal override RestGuildChannelInviteActor Actor { get; }

    public RestGuildChannelInvite(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildChannelIdentity channel,
        IInviteModel model,
        RestGuildChannelInviteActor? actor = null,
        GuildScheduledEventIdentity? scheduledEvent = null
    ) : base(client, guild, model, actor, scheduledEvent)
    {
        Actor = actor ?? new(client, guild, channel, GuildChannelInviteIdentity.Of(this));
    }

    public static RestGuildChannelInvite Construct(DiscordRestClient client, Context context, IInviteModel model)
        => new(
            client,
            context.Guild,
            context.Channel,
            model,
            scheduledEvent: context.ScheduledEvent
        );
}
