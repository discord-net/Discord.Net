using Discord.Models;
using Discord.Rest;
using Discord.Rest.Extensions;
using Discord.Stage;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestStageInstanceActor :
    RestActor<RestStageInstanceActor, ulong, RestStageInstance, IStageInstanceModel>,
    IStageInstanceActor
{
    [SourceOfTruth] public RestStageChannelActor Channel { get; }

    [SourceOfTruth] public RestGuildActor Guild { get; }

    internal override StageInstanceIdentity Identity { get; }

    public RestStageInstanceActor(
        DiscordRestClient client,
        GuildIdentity guild,
        StageChannelIdentity channel,
        StageInstanceIdentity instance
    ) : base(client, instance)
    {
        Identity = instance | this;

        Guild = client.Guilds[guild];
        Channel = Guild.Channels.Stage[channel];
    }

    [SourceOfTruth]
    internal override RestStageInstance CreateEntity(IStageInstanceModel model)
        => RestStageInstance.Construct(Client, this, model);
}

public sealed partial class RestStageInstance :
    RestEntity<ulong>,
    IStageInstance,
    IRestConstructable<RestStageInstance, RestStageInstanceActor, IStageInstanceModel>
{
    public string Topic => Model.Topic;

    public StagePrivacyLevel PrivacyLevel => (StagePrivacyLevel) Model.PrivacyLevel;

    [SourceOfTruth] public RestGuildScheduledEventActor? Event { get; private set; }

    [ProxyInterface(typeof(IStageInstanceActor))]
    internal RestStageInstanceActor Actor { get; }

    internal IStageInstanceModel Model { get; private set; }

    internal RestStageInstance(
        DiscordRestClient client,
        IStageInstanceModel model,
        RestStageInstanceActor actor
    ) : base(client, model.Id)
    {
        Actor = actor;
        Model = model;
    }

    public static RestStageInstance Construct(
        DiscordRestClient client,
        RestStageInstanceActor actor,
        IStageInstanceModel model
    ) => new(client, model, actor);

    public ValueTask UpdateAsync(IStageInstanceModel model, CancellationToken token = default)
    {
        Event = Event.UpdateFrom(
            model.EventId,
            RestGuildScheduledEventActor.Factory,
            Client,
            Guild.Identity
        );

        Model = model;

        return ValueTask.CompletedTask;
    }

    public IStageInstanceModel GetModel() => Model;
}