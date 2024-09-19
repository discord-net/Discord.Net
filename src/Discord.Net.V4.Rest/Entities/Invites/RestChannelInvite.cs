using Discord.Models;

namespace Discord.Rest;

public partial class RestChannelInviteActor :
    RestInviteActor,
    IChannelInviteActor,
    IRestActor<string, RestChannelInvite, ChannelInviteIdentity, IInviteModel>
{
    [SourceOfTruth] public RestChannelActor Channel { get; }

    [SourceOfTruth] internal override ChannelInviteIdentity Identity { get; }

    [TypeFactory]
    public RestChannelInviteActor(
        DiscordRestClient client,
        ChannelIdentity channel,
        ChannelInviteIdentity invite
    ) : base(client, invite)
    {
        Identity = invite | this;
        Channel = channel.Actor ?? client.Channels[channel.Id];
    }

    [SourceOfTruth]
    internal override RestChannelInvite CreateEntity(IInviteModel model)
        => RestChannelInvite.Construct(Client, this, model);
}

public partial class RestChannelInvite :
    RestInvite,
    IChannelInvite,
    IRestConstructable<RestChannelInvite, RestChannelInviteActor, IInviteModel>
{
    [ProxyInterface] internal override RestChannelInviteActor Actor { get; }

    internal RestChannelInvite(
        DiscordRestClient client,
        IInviteModel model,
        RestChannelInviteActor actor
    ) : base(client, model, actor)
    {
        Actor = actor;
    }

    public static RestChannelInvite Construct(
        DiscordRestClient client,
        RestChannelInviteActor actor,
        IInviteModel model
    ) => new(client, model, actor);
}