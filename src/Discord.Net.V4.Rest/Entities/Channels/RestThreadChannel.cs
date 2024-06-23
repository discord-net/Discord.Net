using Discord.Entities.Channels.Threads;
using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;
using PropertyChanged;
using System.Collections.Immutable;

namespace Discord.Rest;

public sealed partial class RestLoadableThreadChannelActor(DiscordRestClient client, ulong guildId, ulong id, IThreadChannelModel? model = null) :
    RestThreadChannelActor(client, guildId, id),
    ILoadableThreadActor
{
    [ProxyInterface(typeof(ILoadableEntity<IThreadChannel>))]
    public RestLoadable<ulong, RestThreadChannel, IThreadChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            id,
            Routes.GetChannel(id),
            EntityUtils.FactoryOfDescendantModel<ulong, IChannelModel, RestThreadChannel, ThreadChannelBase>(
                (_, thread) => RestThreadChannel.Construct(client, thread, new(guildId))
            ),
            model
        );
}

[ExtendInterfaceDefaults(
    typeof(IThreadActor),
    typeof(IModifiable<ulong, IThreadActor, ModifyThreadChannelProperties, ModifyThreadChannelParams>)
)]
public partial class RestThreadChannelActor(
    DiscordRestClient client,
    ulong guildId,
    ulong id,
    IThreadChannelModel? model = null,
    IThreadMemberModel? currentThreadMember = null
):
    RestGuildChannelActor(client, guildId, id),
    IThreadActor,
    IActor<ulong, RestThreadChannel>
{
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, guildId, id);

    public RestLoadableThreadMemberActor CurrentThreadMember { get; } =
        new(client, guildId, id, client.SelfUser.Id, model: currentThreadMember, thread: model);

    public IEnumerableIndexableActor<ILoadableThreadMemberActor, ulong, IThreadMember> ThreadMembers => throw new NotImplementedException();

    ILoadableThreadMemberActor IThreadActor.CurrentThreadMember => CurrentThreadMember;
}

public partial class RestThreadChannel(
    DiscordRestClient client,
    ulong guildId,
    IThreadChannelModel model,
    RestThreadChannelActor? actor = null,
    IThreadMemberModel? currentThreadMember = null
):
    RestGuildChannel(client, guildId, model, actor),
    IThreadChannel,
    IContextConstructable<RestThreadChannel, IThreadChannelModel, RestThreadChannel.Context, DiscordRestClient>
{
    public readonly record struct Context(
        ulong GuildId,
        IThreadMemberModel? CurrentThreadMember = null
    );

    [OnChangedMethod(nameof(OnModelUpdated))]
    internal new IThreadChannelModel Model { get; } = model;

    [ProxyInterface(typeof(IThreadActor), typeof(IThreadMemberRelationship), typeof(IMessageChannelActor))]
    internal override RestThreadChannelActor Actor { get; }
        = actor ?? new(
            client,
            guildId,
            model.Id,
            model,
            currentThreadMember ?? model.GetReferencedEntityModel<ulong, IThreadMemberModel>(client.SelfUser.Id));

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

    public RestLoadableGuildChannel Parent { get; } = new(client, guildId, model.ParentId!.Value);

    public RestLoadableUserActor? Owner { get; private set; } = model.OwnerId.HasValue ? new(client, model.OwnerId.Value) : null;

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

    public IReadOnlyCollection<ulong> AppliedTags { get; private set; } = model.AppliedTags.ToImmutableArray();

    public DateTimeOffset CreatedAt => Model.CreatedAt ?? SnowflakeUtils.FromSnowflake(Id);

    #endregion

    ILoadableUserActor? IThreadChannel.Owner => Owner;
    ILoadableGuildChannelActor IThreadChannel.Parent => Parent;
}
