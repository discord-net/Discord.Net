using Discord.Entities.Channels.Threads;
using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Channels;
using Discord.Rest.Extensions;
using Discord.Rest.Guilds;
using System.Collections.Immutable;

namespace Discord.Rest;

[method: TypeFactory(LastParameter = nameof(thread))]
public sealed partial class RestLoadableThreadChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ThreadIdentity thread,
    ThreadMemberIdentity? currentThreadMember = null
) :
    RestThreadChannelActor(client, guild, thread, currentThreadMember),
    ILoadableThreadChannelActor
{
    [RestLoadableActorSource]
    [ProxyInterface(typeof(ILoadableEntity<IThreadChannel>))]
    public RestLoadable<ulong, RestThreadChannel, IThreadChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            thread,
            Routes.GetChannel(thread.Id),
            EntityUtils.FactoryOfDescendantModel<ulong, IChannelModel, RestThreadChannel, IThreadChannelModel>(
                (_, thread) => RestThreadChannel.Construct(client, thread, new(guild))
            ).Invoke
        );
}

[ExtendInterfaceDefaults]
public partial class RestThreadChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ThreadIdentity thread,
    ThreadMemberIdentity? currentThreadMember = null
) :
    RestGuildChannelActor(client, guild, thread),
    IThreadChannelActor,
    IActor<ulong, RestThreadChannel>
{
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, thread);

    [SourceOfTruth]
    public RestLoadableThreadMemberActor CurrentThreadMember { get; } =
        new(client, guild, thread, currentThreadMember ?? ThreadMemberIdentity.Of(client.SelfUser.Id));

    public IEnumerableIndexableActor<ILoadableThreadMemberActor, ulong, IThreadMember> ThreadMembers =>
        throw new NotImplementedException();

    public ILoadableThreadableChannelActor Parent => throw new NotImplementedException();

    [SourceOfTruth]
    [CovariantOverride]
    internal RestThreadChannel CreateEntity(IThreadChannelModel model)
        => RestThreadChannel.Construct(Client, model, new RestThreadChannel.Context(
            Guild.Identity,
            CurrentThreadMember.Identity
        ));
}

public partial class RestThreadChannel :
    RestGuildChannel,
    IThreadChannel,
    IContextConstructable<RestThreadChannel, IThreadChannelModel, RestThreadChannel.Context, DiscordRestClient>
{
    public readonly record struct Context(
        GuildIdentity? Guild = null,
        ThreadMemberIdentity? CurrentThreadMember = null,
        ThreadableChannelIdentity? Parent = null
    );

    #region Properties

    public RestLoadableThreadableChannelActor Parent { get; }

    public RestLoadableUserActor? Owner { get; private set; }

    public new ThreadType Type => (ThreadType)Model.Type;

    public bool HasJoined => Model.HasJoined;

    public bool IsArchived => Model.IsArchived;

    public ThreadArchiveDuration AutoArchiveDuration => (ThreadArchiveDuration)Model.AutoArchiveDuration;

    public DateTimeOffset ArchiveTimestamp => Model.ArchiveTimestamp;

    public bool IsLocked => Model.IsLocked;

    public int MemberCount => Model.MemberCount;

    public int MessageCount => Model.MessageCount;

    public bool? IsInvitable => Model.IsInvitable;

    public IReadOnlyCollection<ulong> AppliedTags { get; private set; }

    public DateTimeOffset CreatedAt => Model.CreatedAt ?? SnowflakeUtils.FromSnowflake(Id);

    #endregion

    [ProxyInterface(
        typeof(IThreadChannelActor),
        typeof(IThreadMemberRelationship),
        typeof(IMessageChannelActor),
        typeof(IEntityProvider<IThreadChannel, IThreadChannelModel>)
    )]
    internal override RestThreadChannelActor Actor { get; }

    internal override IThreadChannelModel Model => _model;

    private IThreadChannelModel _model;

    internal RestThreadChannel(
        DiscordRestClient client,
        GuildIdentity guild,
        IThreadChannelModel model,
        RestThreadChannelActor? actor = null,
        ThreadMemberIdentity? currentThreadMember = null,
        ThreadableChannelIdentity? parent = null
    ) : base(client, guild, model, actor)
    {
        _model = model;
        Actor = actor ?? new(
            client,
            guild,
            ThreadIdentity.Of(this),
            currentThreadMember ?? GetCurrentThreadMemberIdentity(client, guild, ThreadIdentity.Of(this), model)
        );
        Parent = new(
            client,
            guild,
            parent ?? ThreadableChannelIdentity.Of(model.ParentId)
        );
        Owner = model.OwnerId.HasValue ? new(client, UserIdentity.Of(model.OwnerId.Value)) : null;
        AppliedTags = model.AppliedTags.ToImmutableArray();
    }


    public static RestThreadChannel Construct(DiscordRestClient client, IThreadChannelModel model, Context context)
        => new(client, context.Guild ?? GuildIdentity.Of(model.GuildId), model, currentThreadMember: context.CurrentThreadMember, parent: context.Parent);

    [CovariantOverride]
    public ValueTask UpdateAsync(IThreadChannelModel model, CancellationToken token = default)
    {
        if (!AppliedTags.SequenceEqual(model.AppliedTags))
            AppliedTags = model.AppliedTags.ToImmutableArray();

        Parent.Loadable.Id = model.ParentId!;

        Owner = Owner.UpdateFrom(
            model.OwnerId,
            RestLoadableUserActor.Factory,
            UserIdentity.Of,
            Client
        );

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IThreadChannelModel GetModel() => Model;

    internal static ThreadMemberIdentity GetCurrentThreadMemberIdentity(
        DiscordRestClient client,
        GuildIdentity guild,
        ThreadIdentity thread,
        IThreadChannelModel? model)
    {
        return ThreadMemberIdentity.OfNullable(
            model?.GetReferencedEntityModel<ulong, IThreadMemberModel>(client.SelfUser.Id),
            model => RestThreadMember.Construct(client, model, new RestThreadMember.Context(
                guild,
                thread
            ))
        ) ?? ThreadMemberIdentity.Of(client.SelfUser.Id);
    }

    ILoadableUserActor? IThreadChannel.Owner => Owner;
    ILoadableThreadableChannelActor IThreadableRelationship.Parent => Parent;
}
