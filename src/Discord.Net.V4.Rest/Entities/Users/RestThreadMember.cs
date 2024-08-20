using Discord.Models;
using Discord.Rest;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestThreadMemberActor :
    RestActor<ulong, RestThreadMember, ThreadMemberIdentity, IThreadMemberModel>,
    IThreadMemberActor
{
    [SourceOfTruth] public RestThreadChannelActor Thread { get; }

    [SourceOfTruth] public RestMemberActor Member { get; }

    [SourceOfTruth] public RestUserActor User { get; }

    internal override ThreadMemberIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(threadMember))]
    public RestThreadMemberActor(
        DiscordRestClient client,
        GuildIdentity guild,
        ThreadIdentity thread,
        ThreadMemberIdentity threadMember,
        MemberIdentity? member = null,
        UserIdentity? user = null
    ) : base(client, threadMember)
    {
        Identity = threadMember | this;

        Thread = thread.Actor ?? new(client, guild, thread, threadMember);
        User = user?.Actor ?? new(client, user ?? UserIdentity.Of(threadMember.Id));
        Member = member?.Actor ?? new(client, guild, member ?? MemberIdentity.Of(threadMember.Id), user | User);
    }

    [SourceOfTruth]
    internal override RestThreadMember CreateEntity(IThreadMemberModel model)
        => RestThreadMember.Construct(
            Client,
            this,
            model
        );
}

public sealed partial class RestThreadMember :
    RestEntity<ulong>,
    IThreadMember,
    IRestConstructable<RestThreadMember, RestThreadMemberActor, IThreadMemberModel>
{
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
        IThreadMemberModel model,
        RestThreadMemberActor actor
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor;
    }

    public static RestThreadMember Construct(
        DiscordRestClient client,
        RestThreadMemberActor actor,
        IThreadMemberModel model
    ) => new(
        client,
        model,
        actor
    );

    public ValueTask UpdateAsync(IThreadMemberModel model, CancellationToken token = default)
    {
        Model = model;
        return ValueTask.CompletedTask;
    }

    public IThreadMemberModel GetModel() => Model;
}