using Discord.Models;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;

namespace Discord.Rest;

public sealed partial class RestLoadableThreadMemberActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ThreadIdentity thread,
    ThreadMemberIdentity threadMember,
    MemberIdentity? member = null,
    UserIdentity? user = null
) :
    RestThreadMemberActor(client, guild, thread, threadMember, member, user),
    ILoadableThreadMemberActor
{
    internal ThreadMemberIdentity Identity { get; } = threadMember;

    [ProxyInterface(typeof(ILoadableEntity<IThreadMember>))]
    internal RestLoadable<ulong, RestThreadMember, IThreadMember, IThreadMemberModel> Loadable { get; }
        = RestLoadable<ulong, RestThreadMember, IThreadMember, IThreadMemberModel>
            .FromContextConstructable<RestThreadMember, RestThreadMember.Context>(
                client,
                threadMember,
                Routes.GetThreadMember(thread.Id, threadMember.Id, true),
                new RestThreadMember.Context(guild, thread, member, user)
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
) :
    RestActor<ulong, RestThreadMember>(client, threadMember.Id),
    IThreadMemberActor
{
    public RestLoadableThreadChannelActor Thread { get; } =
        new(client, guild, thread, ThreadMemberIdentity.Of(client.SelfUser.Id));

    public RestLoadableGuildMemberActor Member { get; } =
        new(client, guild, member ?? MemberIdentity.Of(threadMember.Id));

    public RestLoadableUserActor User { get; } =
        new(client, user ?? UserIdentity.Of(threadMember.Id));

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

    public DateTimeOffset JoinedAt => Model.JoinTimestamp;

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
        Actor = actor ?? new(
            client,
            guild,
            thread,
            ThreadMemberIdentity.Of(this),
            member,
            user
        );
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

    public ValueTask UpdateAsync(IThreadMemberModel model, CancellationToken token = default)
    {
        Model = model;
        return ValueTask.CompletedTask;
    }

    public IThreadMemberModel GetModel() => Model;
}
