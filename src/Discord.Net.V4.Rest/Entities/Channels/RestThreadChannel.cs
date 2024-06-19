using Discord.Entities.Channels.Threads;
using Discord.Models;
using Discord.Models.Json;
using System.Collections.Immutable;

namespace Discord.Rest.Channels;

public sealed partial class RestLoadableThreadChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestThreadChannelActor(client, guildId, id),
    ILoadableThreadActor
{
    [ProxyInterface(typeof(ILoadableEntity<IThreadChannel>))]
    public RestLoadable<ulong, RestThreadChannel, IThreadChannel, Channel> Loadable { get; } =
        new(
            client,
            id,
            Routes.GetChannel(id),
            EntityUtils.FactoryOfDescendantModel<ulong, Channel, RestThreadChannel, ThreadChannelBase>(
                (_, thread) => RestThreadChannel.Construct(client, thread, guildId)
            )
        );
}

[ExtendInterfaceDefaults(
    typeof(IThreadActor),
    typeof(IModifiable<ulong, IThreadActor, ModifyThreadChannelProperties, ModifyThreadChannelParams>),
    typeof(IMessageChannelActor)
)]
public partial class RestThreadChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestGuildChannelActor(client, guildId, id),
    IThreadActor
{
    public ILoadableThreadMemberActor<IThreadMember> CurrentThreadMember => throw new NotImplementedException();

    public ILoadableRootActor<ILoadableThreadMemberActor<IThreadMember>, ulong, IThreadMember> ThreadMembers => throw new NotImplementedException();

    public IRootActor<ILoadableMessageActor<IMessage>, ulong, IMessage> Messages => throw new NotImplementedException();
}

public partial class RestThreadChannel(DiscordRestClient client, ulong guildId, IThreadChannelModel model, RestThreadChannelActor? actor = null) :
    RestGuildChannel(client, guildId, model, actor),
    IThreadChannel,
    IContextConstructable<RestThreadChannel, IThreadChannelModel, ulong, DiscordRestClient>
{
    internal override IThreadChannelModel Model { get; } = model;

    [ProxyInterface(typeof(IThreadActor), typeof(IThreadMemberRelationship), typeof(IMessageChannelActor))]
    internal override RestThreadChannelActor Actor { get; } = actor ?? new(client, guildId, model.Id);

    public static RestThreadChannel Construct(DiscordRestClient client, IThreadChannelModel model, ulong context)
        => new(client, context, model);

    #region Loadables

    public ILoadableGuildChannelActor Parent => throw new NotImplementedException();

    public ILoadableEntity<ulong, IUser> Owner => throw new NotImplementedException();

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

    public IReadOnlyCollection<ulong> AppliedTags => Model.AppliedTags.ToImmutableArray();

    public DateTimeOffset CreatedAt => Model.CreatedAt ?? SnowflakeUtils.FromSnowflake(Id);

    #endregion
}
