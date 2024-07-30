using Discord.Entities.Channels.Threads;
using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Discord.Rest.Extensions;
using System.Collections.Immutable;

namespace Discord.Rest;

using EnumerableThreadMembers = RestEnumerableIndexableActor<RestThreadMemberActor, ulong, RestThreadMember, IThreadMember, IEnumerable<IThreadMemberModel>>;

[ExtendInterfaceDefaults]
public partial class RestThreadChannelActor :
    RestGuildChannelActor,
    IThreadChannelActor,
    IRestActor<ulong, RestThreadChannel, ThreadIdentity>
{
    [method: TypeFactory]
    public RestThreadChannelActor(DiscordRestClient client,
        GuildIdentity guild,
        ThreadIdentity thread,
        ThreadMemberIdentity? currentThreadMember = null) : base(client, guild, thread)
    {
        thread = Identity = thread.MostSpecific(this);

        MessageChannelActor = new RestMessageChannelActor(client, thread);

        currentThreadMember ??= ThreadMemberIdentity.Of(client.CurrentUser.Id);

        CurrentThreadMember = currentThreadMember.Actor ?? new(
            client,
            guild,
            thread,
            currentThreadMember
        );

        ThreadMembers = RestActors.Fetchable(
            Template.Of<RestThreadMemberActor>(),
            client,
            RestThreadMemberActor.Factory,
            guild,
            thread,
            entityFactory: RestThreadMember.Construct,
            new RestThreadMember.Context(guild, thread),
            IThreadMember.FetchManyRoute(this)
        );
    }

    public override ThreadIdentity Identity { get; }

    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; }

    [SourceOfTruth]
    public RestThreadMemberActor CurrentThreadMember { get; }

    [SourceOfTruth]
    public EnumerableThreadMembers ThreadMembers { get; }


    [SourceOfTruth]
    [CovariantOverride]
    internal RestThreadChannel CreateEntity(IThreadChannelModel model)
        => RestThreadChannel.Construct(Client, new RestThreadChannel.Context(
            Guild.Identity,
            CurrentThreadMember.Identity
        ), model);
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

    [SourceOfTruth] public RestThreadableChannelActor Parent { get; }

    [SourceOfTruth] public RestUserActor Creator { get; }

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

    [ProxyInterface(typeof(IThreadChannelActor))]
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

        Creator = new RestUserActor(client, UserIdentity.Of(model.OwnerId));

        AppliedTags = model.AppliedTags.ToImmutableArray();
    }


    public static RestThreadChannel Construct(DiscordRestClient client, Context context, IThreadChannelModel model)
        => new(client, context.Guild ?? GuildIdentity.Of(model.GuildId), model,
            currentThreadMember: context.CurrentThreadMember, parent: context.Parent);

    [CovariantOverride]
    public ValueTask UpdateAsync(IThreadChannelModel model, CancellationToken token = default)
    {
        if (!AppliedTags.SequenceEqual(model.AppliedTags))
            AppliedTags = model.AppliedTags.ToImmutableArray();

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
            model?.GetReferencedEntityModel<ulong, IThreadMemberModel>(client.CurrentUser.Id),
            model => RestThreadMember.Construct(client, new RestThreadMember.Context(
                guild,
                thread
            ), model)
        ) ?? ThreadMemberIdentity.Of(client.CurrentUser.Id);
    }
}
