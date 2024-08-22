using Discord.Models;
using Discord.Rest;
using Discord.Rest.Extensions;
using Discord.Stage;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestStageInstanceActor :
    RestActor<ulong, RestStageInstance, StageInstanceIdentity>,
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

        Guild = guild.Actor ?? new(client, guild);
        Channel = channel.Actor ?? new(client, Guild.Identity, channel);
    }

    [SourceOfTruth]
    internal RestStageInstance CreateEntity(IStageInstanceModel model)
        => RestStageInstance.Construct(Client, new(Guild.Identity, Channel.Identity), model);
}

public sealed partial class RestStageInstance :
    RestEntity<ulong>,
    IStageInstance,
    IRestConstructable<RestStageInstance, RestStageInstanceActor, IStageInstanceModel>
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
