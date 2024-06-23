using Discord.Models;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;

namespace Discord.Rest;

public sealed partial class RestLoadableThreadMemberActor(
    DiscordRestClient client,
    ulong guildId,
    ulong threadId,
    ulong id,
    IThreadMemberModel? model = null,
    IThreadChannelModel? thread = null,
    IMemberModel? member = null,
    IUserModel? user = null
):
    RestThreadMemberActor(client, guildId, threadId, id, thread, member, user),
    ILoadableThreadMemberActor
{
    [ProxyInterface(typeof(ILoadableEntity<IThreadMember>))]
    public RestLoadable<ulong, RestThreadMember, IThreadMember, IThreadMemberModel> Loadable { get; }
        = RestLoadable<ulong, RestThreadMember, IThreadMember, IThreadMemberModel>
            .FromContextConstructable<RestThreadMember, RestThreadMember.Context>(
                client,
                id,
                Routes.GetThreadMember(threadId, id, true),
                new RestThreadMember.Context(guildId, threadId, thread, member, user),
                model
            );
}

[ExtendInterfaceDefaults(typeof(IThreadMemberActor))]
public partial class RestThreadMemberActor(
    DiscordRestClient client,
    ulong guildId,
    ulong threadId,
    ulong id,
    IThreadChannelModel? thread = null,
    IMemberModel? member = null,
    IUserModel? user = null
):
    RestActor<ulong, RestThreadMember>(client, id),
    IThreadMemberActor
{
    public RestLoadableThreadChannelActor Thread { get; } =
        new(client, guildId, threadId, thread);

    public RestLoadableGuildMemberActor Member { get; } =
        new(client, guildId, id, member);

    public RestLoadableUserActor User { get; } =
        new(client, id, user);

    ILoadableThreadActor IThreadRelationship.Thread => Thread;
    ILoadableGuildMemberActor IMemberRelationship.Member => Member;
    ILoadableUserActor IUserRelationship.User => User;
}

public sealed partial class RestThreadMember(
    DiscordRestClient client,
    ulong guildId,
    ulong threadId,
    IThreadMemberModel model,
    RestThreadMemberActor? actor = null,
    IThreadChannelModel? thread = null,
    IMemberModel? member = null,
    IUserModel? user = null
):
    RestEntity<ulong>(client, model.Id),
    IThreadMember,
    IContextConstructable<RestThreadMember, IThreadMemberModel, RestThreadMember.Context, DiscordRestClient>
{
    public readonly record struct Context(
        ulong GuildId,
        ulong ThreadId,
        IThreadChannelModel? Thread = null,
        IMemberModel? Member = null,
        IUserModel? User = null
    );

    [ProxyInterface(
        typeof(IThreadMemberActor),
        typeof(IThreadRelationship),
        typeof(IMemberRelationship),
        typeof(IUserRelationship)
    )]
    internal RestThreadMemberActor Actor { get; } = actor ?? new(client, guildId, threadId, model.Id, thread, member, user);

    public static RestThreadMember Construct(DiscordRestClient client, IThreadMemberModel model, Context context)
        => new(
            client,
            context.GuildId,
            context.ThreadId,
            model,
            thread: context.Thread,
            member: context.Member,
            user: context.User
        );

    public DateTimeOffset JoinedAt => model.JoinTimestamp;
}
