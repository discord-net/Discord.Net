using Discord.Models;

namespace Discord.Rest;

public partial class RestPrivateThreadChannelActor :
    RestThreadChannelActor,
    IPrivateThreadChannelActor,
    IRestActor<RestPrivateThreadChannelActor, ulong, RestPrivateThreadChannel, IThreadChannelModel>
{
    [SourceOfTruth] internal override PrivateThreadChannelIdentity Identity { get; }

    public RestPrivateThreadChannelActor(
        DiscordRestClient client,
        PrivateThreadChannelIdentity thread
    ) : base(client, thread)
    {
        Identity = thread;
    }

    [SourceOfTruth]
    internal override RestPrivateThreadChannel CreateEntity(IThreadChannelModel model)
        => RestPrivateThreadChannel.Construct(Client, this, model);
}

public partial class RestPrivateThreadChannel :
    RestThreadChannel,
    IPrivateThreadChannel,
    IRestConstructable<RestPrivateThreadChannel, RestPrivateThreadChannelActor, IThreadChannelModel>
{
    [ProxyInterface(typeof(IPrivateThreadChannelActor))]
    internal override RestPrivateThreadChannelActor ThreadActor { get; }

    internal RestPrivateThreadChannel(
        DiscordRestClient client,
        IThreadChannelModel model,
        RestPrivateThreadChannelActor actor
    ) : base(client, model, actor)
    {
        ThreadActor = actor;
    }

    public static RestPrivateThreadChannel Construct(
        DiscordRestClient client,
        RestPrivateThreadChannelActor actor,
        IThreadChannelModel model
    ) => new(client, model, actor);
}