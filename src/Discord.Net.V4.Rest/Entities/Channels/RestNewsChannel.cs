using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestNewsChannelActor :
    RestTextChannelActor,
    IAnnouncementChannelActor,
    IRestActor<RestNewsChannelActor, ulong, RestNewsChannel, IGuildNewsChannelModel>
{
    [SourceOfTruth] internal override NewsChannelIdentity Identity { get; }

    [TypeFactory]
    public RestNewsChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        NewsChannelIdentity channel
    ) : base(client, guild, channel)
    {
        Identity = channel | this;
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal RestNewsChannel CreateEntity(IGuildNewsChannelModel model)
        => RestNewsChannel.Construct(Client, this, model);
}

public partial class RestNewsChannel :
    RestTextChannel,
    IAnnouncementChannel,
    IRestConstructable<RestNewsChannel, RestNewsChannelActor, IGuildNewsChannelModel>
{
    internal override IGuildNewsChannelModel Model => _model;

    [ProxyInterface(
        typeof(IAnnouncementChannelActor),
        typeof(IEntityProvider<IAnnouncementChannel, IGuildNewsChannelModel>)
    )]
    internal override RestNewsChannelActor Actor { get; }

    private IGuildNewsChannelModel _model;

    internal RestNewsChannel(
        DiscordRestClient client,
        IGuildNewsChannelModel model,
        RestNewsChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor;
    }

    public static RestNewsChannel Construct(
        DiscordRestClient client,
        RestNewsChannelActor actor,
        IGuildNewsChannelModel model
    ) => new(client, model, actor);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildNewsChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override IGuildNewsChannelModel GetModel() => Model;
}