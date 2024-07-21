using Discord.Models;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;

namespace Discord.Rest;

[method: TypeFactory(LastParameter = nameof(threadMember))]
[ExtendInterfaceDefaults(typeof(IThreadMemberActor))]
public sealed partial class RestThreadMemberActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ThreadIdentity thread,
    ThreadMemberIdentity threadMember,
    MemberIdentity? member = null,
    UserIdentity? user = null
):
    RestActor<ulong, RestThreadMember, ThreadMemberIdentity>(client, threadMember),
    IThreadMemberActor
{
    [SourceOfTruth]
    public RestThreadChannelActor Thread { get; } =
        thread.Actor ?? new(client, guild, thread, threadMember);

    [SourceOfTruth]
    public RestGuildMemberActor Member { get; } =
        member?.Actor ?? new(client, guild, member ?? MemberIdentity.Of(threadMember.Id));

    [SourceOfTruth]
    public RestUserActor User { get; } =
        user?.Actor ?? new(client, user ?? UserIdentity.Of(threadMember.Id));

    [SourceOfTruth]
    internal RestThreadMember CreateEntity(IThreadMemberModel model)
        => RestThreadMember.Construct(Client, new RestThreadMember.Context(
            guild, thread, member, user
        ), model);
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
        typeof(IUserRelationship),
        typeof(IEntityProvider<IThreadMember, IThreadMemberModel>)
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

    public static RestThreadMember Construct(DiscordRestClient client, Context context, IThreadMemberModel model)
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
