using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestGuildChannelInviteActor :
    RestInviteActor,
    IGuildChannelInviteActor,
    IRestActor<RestGuildChannelInviteActor, string, RestGuildChannelInvite, IInviteModel>
{
    [SourceOfTruth] public RestGuildChannelActor Channel { get; }
    
    [SourceOfTruth]
    public RestGuildActor Guild { get; }

    [SourceOfTruth] internal override GuildChannelInviteIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(invite))]
    public RestGuildChannelInviteActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildChannelIdentity channel,
        GuildChannelInviteIdentity invite
    ) : base(client, invite)
    {
        Identity = invite | this;
        
        Guild = guild.Actor ?? client.Guilds[guild.Id];
        Channel = channel.Actor ?? Guild.Channels[channel.Id];
    }

    [SourceOfTruth]
    internal override RestGuildChannelInvite CreateEntity(IInviteModel model)
        => RestGuildChannelInvite.Construct(Client, this, model);
}

[ExtendInterfaceDefaults]
public sealed partial class RestGuildChannelInvite :
    RestInvite,
    IGuildChannelInvite,
    IRestConstructable<RestGuildChannelInvite, RestGuildChannelInviteActor, IInviteModel>
{
    [SourceOfTruth]
    public RestGuildScheduledEventActor? GuildScheduledEvent
        => Computed(nameof(GuildScheduledEvent), model => 
            model.ScheduledEventId.HasValue 
                ? Actor.Guild.ScheduledEvents[model.ScheduledEventId.Value]
                : null
        );
    
    [ProxyInterface] internal override RestGuildChannelInviteActor Actor { get; }

    public RestGuildChannelInvite(
        DiscordRestClient client,
        IInviteModel model,
        RestGuildChannelInviteActor actor
    ) : base(client, model, actor)
    {
        Actor = actor;
    }

    public static RestGuildChannelInvite Construct(
        DiscordRestClient client, 
        RestGuildChannelInviteActor actor, 
        IInviteModel model
        ) => new(
            client,
            model,
            actor
        );
}