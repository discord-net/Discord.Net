using Discord.Models;
using Discord.Rest;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestThreadMemberActor :
    RestActor<RestThreadMemberActor, ulong, RestThreadMember, IThreadMemberModel>,
    IThreadMemberActor
{
    [SourceOfTruth] public RestThreadChannelActor Thread { get; }

    [SourceOfTruth] public RestUserActor User { get; }

    internal override ThreadMemberIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(threadMember))]
    public RestThreadMemberActor(
        DiscordRestClient client,
        ThreadIdentity thread,
        ThreadMemberIdentity threadMember
    ) : base(client, threadMember)
    {
        Identity = threadMember | this;

        Thread = thread.Actor ?? client.Threads[thread.Id];
        User = client.Users[threadMember.Id];
    }

    [SourceOfTruth]
    internal override RestThreadMember CreateEntity(IThreadMemberModel model)
        => RestThreadMember.Construct(
            Client,
            this,
            model
        );
}

public sealed partial class RestGuildThreadMemberActor :
    RestThreadMemberActor,
    IGuildThreadMemberActor,
    IRestActor<RestGuildThreadMemberActor, ulong, RestThreadMember, IThreadMemberModel>
{
    [SourceOfTruth]
    public RestGuildActor Guild { get; }
    
    [SourceOfTruth] public RestMemberActor Member { get; }

    [SourceOfTruth] public new RestGuildThreadChannelActor Thread { get; }

    [SourceOfTruth] internal override GuildThreadMemberIdentity Identity { get; }
    
    [TypeFactory(LastParameter = nameof(threadMember))]
    public RestGuildThreadMemberActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildThreadIdentity thread,
        GuildThreadMemberIdentity threadMember,
        MemberIdentity? member = null
    ) : base(client, ThreadIdentity.Of(thread.Id), threadMember)
    {
        Identity = threadMember | this;

        Guild = client.Guilds[guild];
        Thread = Guild.Threads[thread];
        Member = Guild.Members[member | threadMember];
    }
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