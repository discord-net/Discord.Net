using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Collections.Immutable;

namespace Discord.Rest;

using IncomingIntegrationChannelTrait = RestIncomingIntegrationChannelTrait<RestMediaChannelActor, RestMediaChannel, MediaChannelIdentity>;

[ExtendInterfaceDefaults]
public sealed partial class RestMediaChannelActor :
    RestThreadableChannelActor,
    IMediaChannelActor,
    IRestActor<ulong, RestMediaChannel, MediaChannelIdentity>
{
    [ProxyInterface(typeof(IIncomingIntegrationChannelTrait))]
    internal IncomingIntegrationChannelTrait IntegrationChannelActor { get; }

    [SourceOfTruth] internal override MediaChannelIdentity Identity { get; }

    [TypeFactory]
    public RestMediaChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        MediaChannelIdentity channel
    ) : base(client, guild, channel)
    {
        Identity = channel | this;

        IntegrationChannelActor = new(client, this, channel);
    }

    [CovariantOverride]
    [SourceOfTruth]
    internal RestMediaChannel CreateEntity(IGuildMediaChannelModel model)
        => RestMediaChannel.Construct(Client, Guild.Identity, model);
}

public partial class RestMediaChannel :
    RestThreadableChannel,
    IMediaChannel,
    IContextConstructable<RestMediaChannel, IGuildMediaChannelModel, GuildIdentity, DiscordRestClient>
{
    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public ThreadArchiveDuration DefaultAutoArchiveDuration => (ThreadArchiveDuration)Model.DefaultAutoArchiveDuration;

    public IReadOnlyCollection<ForumTag> AvailableTags { get; private set; }

    public int? ThreadCreationSlowmode => Model.DefaultThreadRateLimitPerUser;

    public ILoadableEntity<IEmote> DefaultReactionEmoji => throw new NotImplementedException();

    public SortOrder? DefaultSortOrder => (SortOrder?)Model.DefaultSortOrder;

    [ProxyInterface(typeof(IMediaChannelActor))]
    internal override RestMediaChannelActor Actor { get; }

    internal override IGuildMediaChannelModel Model => _model;

    private IGuildMediaChannelModel _model;

    internal RestMediaChannel(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildMediaChannelModel model,
        RestMediaChannelActor? actor = null
    ) : base(client, guild, model)
    {
        _model = model;

        Actor = actor ?? new(client, guild, MediaChannelIdentity.Of(this));

        AvailableTags = model.AvailableTags
            .Select(x => ForumTag.Construct(client, new ForumTag.Context(guild.Id), x))
            .ToImmutableArray();
    }

    public static RestMediaChannel Construct(DiscordRestClient client,
        GuildIdentity guild, IGuildMediaChannelModel model)
        => new(client, guild, model);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildMediaChannelModel model, CancellationToken token = default)
    {
        if (!_model.AvailableTags.SequenceEqual(model.AvailableTags))
        {
            AvailableTags = model.AvailableTags
                .Select(x => ForumTag.Construct(Client, new ForumTag.Context(Actor.Guild.Id), x))
                .ToImmutableArray();
        }

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildMediaChannelModel GetModel() => Model;
}
