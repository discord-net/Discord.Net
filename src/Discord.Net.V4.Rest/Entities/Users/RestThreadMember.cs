using Discord.Models;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;

namespace Discord.Rest;

using ThreadIdentity = IdentifiableEntityOrModel<ulong, RestThreadChannel, IThreadChannelModel>;
using ThreadMemberIdentity = IdentifiableEntityOrModel<ulong, RestThreadMember, IThreadMemberModel>;

public sealed partial class RestLoadableThreadMemberActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ThreadIdentity thread,
    ThreadMemberIdentity member
):
    RestThreadMemberActor(client, guild, threadId, id, thread, member, user),
    ILoadableThreadMemberActor
{
    internal ThreadMemberIdentity Identity { get; } = member;

    [ProxyInterface(typeof(ILoadableEntity<IThreadMember>))]
    internal RestLoadable<ulong, RestThreadMember, IThreadMember, IThreadMemberModel> Loadable { get; }
        = RestLoadable<ulong, RestThreadMember, IThreadMember, IThreadMemberModel>
            .FromContextConstructable<RestThreadMember, RestThreadMember.Context>(
                client,
                id,
                Routes.GetThreadMember(threadId, id, true),
                new RestThreadMember.Context(guild, threadId, thread, member, user),
                model: model
            );
}

[ExtendInterfaceDefaults(typeof(IThreadMemberActor))]
public partial class RestThreadMemberActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ThreadIdentity thread,
    ThreadMemberIdentity threadMember,
    MemberIdentity? member = null,
    UserIdentity? user = null
):
    RestActor<ulong, RestThreadMember>(client, threadMember.Id),
    IThreadMemberActor
{
    public RestLoadableThreadChannelChannelActor Thread { get; } =
        new(client, guild, thread);

    public RestLoadableGuildMemberActor Member { get; } =
        new(client, guild, member ?? threadMember.Id);

    public RestLoadableUserActor User { get; } =
        new(client, user ?? threadMember.Id);

    ILoadableThreadChannelActor IThreadRelationship.ThreadChannel => Thread;
    ILoadableGuildMemberActor IMemberRelationship.Member => Member;
    ILoadableUserActor IUserRelationship.User => User;
}

public sealed partial class RestThreadMember :
    RestEntity<ulong>,
    IThreadMember,
    IContextConstructable<RestThreadMember, IThreadMemberModel, RestThreadMember.Context, DiscordRestClient>
{
    public readonly record struct Context(
        GuildIdentity Guild,
        ThreadIdentity Thread,
        MemberIdentity? Member = null,
        UserIdentity? User = null
    );

    internal IThreadMemberModel Model { get; private set; }

    [ProxyInterface(
        typeof(IThreadMemberActor),
        typeof(IThreadRelationship),
        typeof(IMemberRelationship),
        typeof(IUserRelationship)
    )]
    internal RestThreadMemberActor Actor { get; }

    internal RestThreadMember(
        DiscordRestClient client,
        GuildIdentity guild,
        ThreadIdentity thread,
        IThreadMemberModel model,
        RestThreadMemberActor? actor = null,
        MemberIdentity? member = null,
        UserIdentity? user = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, guild, thread, this, member, user);
    }

    public ValueTask UpdateAsync(IThreadMemberModel model, CancellationToken token = default)
    {
        Model = model;
        return ValueTask.CompletedTask;
    }

    public static RestThreadMember Construct(DiscordRestClient client, IThreadMemberModel model, Context context)
        => new(
            client,
            context.Guild,
            context.Thread,
            model,
            member: context.Member,
            user: context.User
        );

    public DateTimeOffset JoinedAt => Model.JoinTimestamp;
}
