using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestCategoryChannelActor :
    RestGuildChannelActor,
    ICategoryChannelActor,
    IRestActor<ulong, RestCategoryChannel, CategoryChannelIdentity, IGuildCategoryChannelModel>
{
    [SourceOfTruth] internal override CategoryChannelIdentity Identity { get; }

    [TypeFactory]
    public RestCategoryChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        CategoryChannelIdentity channel
    ) : base(client, guild, channel)
    {
        Identity = channel | this;
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal RestCategoryChannel CreateEntity(IGuildCategoryChannelModel model)
        => RestCategoryChannel.Construct(Client, this, model);
}

public sealed partial class RestCategoryChannel :
    RestGuildChannel,
    ICategoryChannel,
    IRestConstructable<RestCategoryChannel, RestCategoryChannelActor, IGuildCategoryChannelModel>
{
    [ProxyInterface]
    internal override RestCategoryChannelActor Actor { get; }

    internal override IGuildCategoryChannelModel Model => _model;

    private IGuildCategoryChannelModel _model;

    internal RestCategoryChannel(
        DiscordRestClient client,
        IGuildCategoryChannelModel model,
        RestCategoryChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor;
    }

    public static RestCategoryChannel Construct(
        DiscordRestClient client,
        RestCategoryChannelActor actor,
        IGuildCategoryChannelModel model
    ) => new(client, model, actor);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildCategoryChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return ValueTask.CompletedTask;
    }

    public override IGuildCategoryChannelModel GetModel() => Model;
}