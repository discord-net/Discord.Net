using Discord.Entities.Channels.Threads;
using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;
using PropertyChanged;
using System.Collections.Immutable;

namespace Discord.Rest;

public sealed partial class RestLoadableThreadChannelChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ThreadIdentity thread) :
    RestThreadChannelChannelActor(client, guild, thread.Id),
    ILoadableThreadChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IThreadChannel>))]
    public RestLoadable<ulong, RestThreadChannel, IThreadChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            thread,
            Routes.GetChannel(thread.Id),
            EntityUtils.FactoryOfDescendantModel<ulong, IChannelModel, RestThreadChannel, ThreadChannelBase>(
                (_, thread) => RestThreadChannel.Construct(client, thread, new(guild))
            )
        );
}

[ExtendInterfaceDefaults(
    typeof(IThreadChannelActor),
    typeof(IModifiable<ulong, IThreadChannelActor, ModifyThreadChannelProperties, ModifyThreadChannelParams>)
)]
public partial class RestThreadChannelChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ThreadIdentity thread,
    ThreadMemberIdentity currentThreadMember
) :
    RestGuildChannelActor(client, guild, thread),
    IThreadChannelActor,
    IActor<ulong, RestThreadChannel>
{
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, guild, thread);

    public RestLoadableThreadMemberActor CurrentThreadMember { get; } =
        new(client, guild, thread, currentThreadMember);

    public IEnumerableIndexableActor<ILoadableThreadMemberActor, ulong, IThreadMember> ThreadMembers =>
        throw new NotImplementedException();

    public IThreadChannel CreateEntity(IThreadChannelModel model)
        => RestThreadChannel.Construct(Client, model, new RestThreadChannel.Context(
            Guild.Identity,
            CurrentThreadMember.Identity
        ));


    ILoadableThreadMemberActor IThreadChannelActor.CurrentThreadMember => CurrentThreadMember;
}

public partial class RestThreadChannel :
    RestGuildChannel,
    IThreadChannel,
    IContextConstructable<RestThreadChannel, IThreadChannelModel, RestThreadChannel.Context, DiscordRestClient>
{
    public readonly record struct Context(
        GuildIdentity Guild,
        ThreadMemberIdentity? CurrentThreadMember
    );

    [OnChangedMethod(nameof(OnModelUpdated))]
    internal new IThreadChannelModel Model { get; }

    [ProxyInterface(typeof(IThreadChannelActor), typeof(IThreadMemberRelationship), typeof(IMessageChannelActor))]
    internal override RestThreadChannelChannelActor ChannelActor { get; }

    internal RestThreadChannel(DiscordRestClient client,
        GuildIdentity guild,
        IThreadChannelModel model,
        RestThreadChannelChannelActor? actor = null,
        ThreadMemberIdentity? currentThreadMember = null
    ) : base(client, guild, model, actor)
    {
        Model = model;
        ChannelActor = actor ?? new(
            client,
            guild,
            this,
            currentThreadMember
            ?? model
                .GetReferencedEntityModel<ulong, IThreadMemberModel>(client.SelfUser.Id)
                .Identity<ulong, RestThreadMember, IThreadMemberModel>(
                    model => RestThreadMember.Construct(client, model, new RestThreadMember.Context(
                        guild,
                        this
                    ))
                )
            ?? Client.SelfUser.Id
        );
        Parent = new(client, guild, model.ParentId!.Value);
        Owner = model.OwnerId.HasValue ? new(client, model.OwnerId.Value) : null;
        AppliedTags = model.AppliedTags.ToImmutableArray();
    }

    public static RestThreadChannel Construct(DiscordRestClient client, IThreadChannelModel model, Context context)
        => new(client, context.GuildId, model, currentThreadMember: context.CurrentThreadMember);

    private void OnModelUpdated()
    {
        if (!AppliedTags.SequenceEqual(Model.AppliedTags))
            AppliedTags = Model.AppliedTags.ToImmutableArray();

        Parent.Loadable.Id = Model.ParentId!.Value;

        if (Model.OwnerId.HasValue)
            (Owner ??= new(Client, Model.OwnerId.Value)).Loadable.Id = Model.OwnerId.Value;
        else
            Owner = null;
    }

    #region Loadables

    public RestLoadableGuildChannel Parent { get; }

    public RestLoadableUserActor? Owner { get; private set; }

    #endregion

    #region Properties

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

    ILoadableUserActor? IThreadChannel.Owner => Owner;
    ILoadableGuildChannelActor IThreadChannel.Parent => Parent;
}
