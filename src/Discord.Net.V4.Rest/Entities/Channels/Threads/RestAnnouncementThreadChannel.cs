using Discord.Models;

namespace Discord.Rest;

public partial class RestAnnouncementThreadChannelActor :
    RestThreadChannelActor,
    IAnnouncementThreadChannelActor,
    IRestActor<RestAnnouncementThreadChannelActor, ulong, RestAnnouncementThreadChannel, IThreadChannelModel>
{
    [SourceOfTruth] internal override AnnouncementThreadChannelIdentity Identity { get; }

    public RestAnnouncementThreadChannelActor(
        DiscordRestClient client,
        AnnouncementThreadChannelIdentity thread
    ) : base(client, thread)
    {
        Identity = thread;
    }

    [SourceOfTruth]
    internal override RestAnnouncementThreadChannel CreateEntity(IThreadChannelModel model)
        => RestAnnouncementThreadChannel.Construct(Client, this, model);
}

public partial class RestAnnouncementThreadChannel :
    RestThreadChannel,
    IPrivateThreadChannel,
    IRestConstructable<RestAnnouncementThreadChannel, RestAnnouncementThreadChannelActor, IThreadChannelModel>
{
    [ProxyInterface(typeof(IAnnouncementThreadChannelActor))]
    internal override RestAnnouncementThreadChannelActor ThreadActor { get; }

    internal RestAnnouncementThreadChannel(
        DiscordRestClient client,
        IThreadChannelModel model,
        RestAnnouncementThreadChannelActor actor
    ) : base(client, model, actor)
    {
        ThreadActor = actor;
    }

    public static RestAnnouncementThreadChannel Construct(
        DiscordRestClient client,
        RestAnnouncementThreadChannelActor actor,
        IThreadChannelModel model
    ) => new(client, model, actor);
}