using Discord.Models;

namespace Discord.Rest;

public partial class RestPublicThreadChannelActor :
    RestThreadChannelActor,
    IPublicThreadChannelActor,
    IRestActor<RestPublicThreadChannelActor, ulong, RestPublicThreadChannel, IThreadChannelModel>
{
    [SourceOfTruth] internal override PublicThreadChannelIdentity Identity { get; }

    public RestPublicThreadChannelActor(
        DiscordRestClient client,
        PublicThreadChannelIdentity thread
    ) : base(client, thread)
    {
        Identity = thread;
    }

    [SourceOfTruth]
    internal override RestPublicThreadChannel CreateEntity(IThreadChannelModel model)
        => RestPublicThreadChannel.Construct(Client, this, model);
}

public partial class RestPublicThreadChannel :
    RestThreadChannel,
    IPublicThreadChannel,
    IRestConstructable<RestPublicThreadChannel, RestPublicThreadChannelActor, IThreadChannelModel>
{
    [ProxyInterface(typeof(IPublicThreadChannelActor))]
    internal override RestPublicThreadChannelActor ThreadActor { get; }

    internal RestPublicThreadChannel(
        DiscordRestClient client,
        IThreadChannelModel model,
        RestPublicThreadChannelActor actor
    ) : base(client, model, actor)
    {
        ThreadActor = actor;
    }

    public static RestPublicThreadChannel Construct(
        DiscordRestClient client,
        RestPublicThreadChannelActor actor,
        IThreadChannelModel model
    ) => new(client, model, actor);
}