using Discord.Models;
using Discord.Rest.Channels;
using Discord.Rest.Extensions;
using Discord.Rest.Guilds;
using Discord.Stage;

namespace Discord.Rest.Stage;

public sealed partial class RestStageInstanceActor(
    DiscordRestClient client,
    GuildIdentity guild,
    StageChannelIdentity channel,
    StageInstanceIdentity instance
) :
    RestActor<ulong, RestStageInstance, StageInstanceIdentity>(client, instance),
    IStageInstanceActor
{
    [SourceOfTruth] public RestStageChannelActor Channel { get; } = new(client, guild, channel);

    [SourceOfTruth] public RestGuildActor Guild { get; } = new(client, guild);

    [SourceOfTruth]
    internal RestStageInstance CreateEntity(IStageInstanceModel model)
        => RestStageInstance.Construct(Client, new(Guild.Identity, Channel.Identity), model);
}

public sealed partial class RestStageInstance :
    RestEntity<ulong>,
    IStageInstance,
    IContextConstructable<RestStageInstance, IStageInstanceModel, RestStageInstance.Context, DiscordRestClient>
{
    public readonly record struct Context(GuildIdentity Guild, StageChannelIdentity Channel);

    public string Topic => Model.Topic;

    public StagePrivacyLevel PrivacyLevel => (StagePrivacyLevel)Model.PrivacyLevel;

    [SourceOfTruth] public RestGuildScheduledEventActor? Event { get; private set; }

    [ProxyInterface(typeof(IStageInstanceActor))]
    internal RestStageInstanceActor Actor { get; }

    internal IStageInstanceModel Model { get; private set; }

    internal RestStageInstance(
        DiscordRestClient client,
        GuildIdentity guild,
        StageChannelIdentity channel,
        IStageInstanceModel model
    ) : base(client, model.Id)
    {
        Actor = new(client, guild, channel, StageInstanceIdentity.Of(this));
        Model = model;
    }

    public static RestStageInstance Construct(DiscordRestClient client, Context context, IStageInstanceModel model)
        => new(client, context.Guild, context.Channel, model);

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
