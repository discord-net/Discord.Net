using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

public sealed partial class GatewayThreadMemberActor :
    GatewayCachedActor<ulong, GatewayThreadMember, ThreadMemberIdentity, IThreadMemberModel>,
    IThreadMemberActor
{
    [SourceOfTruth, StoreRoot] public GatewayThreadChannelActor Thread { get; }

    [SourceOfTruth] public GatewayMemberActor Member { get; }

    [SourceOfTruth] public GatewayUserActor User { get; }

    public GatewayThreadMemberActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        ThreadChannelIdentity thread,
        ThreadMemberIdentity threadMember,
        MemberIdentity? member = null,
        UserIdentity? user = null
    ) : base(client, threadMember)
    {
        User = user?.Actor ?? new GatewayUserActor(client, user ?? UserIdentity.Of(threadMember.Id));
        Member = member?.Actor ?? new(client, guild, member ?? MemberIdentity.Of(threadMember.Id), user | User);
        Thread = thread.Actor ?? new(client, guild, thread);
    }

    [SourceOfTruth]
    internal GatewayThreadMember CreateEntity(IThreadMemberModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewayThreadMember :
    GatewayCacheableEntity<GatewayThreadMember, ulong, IThreadMemberModel>,
    IThreadMember
{
    public DateTimeOffset JoinedAt => Model.JoinTimestamp;

    [ProxyInterface] internal GatewayThreadMemberActor Actor { get; }

    internal IThreadMemberModel Model { get; private set; }

    public GatewayThreadMember(
        DiscordGatewayClient client,
        GuildIdentity guild,
        ThreadChannelIdentity thread,
        IThreadMemberModel model,
        MemberIdentity? member = null,
        UserIdentity? user = null,
        GatewayThreadMemberActor? actor = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, guild, thread, ThreadMemberIdentity.Of(this), member, user);
    }

    public static GatewayThreadMember Construct(
        DiscordGatewayClient client,
        ICacheConstructionContext context,
        IThreadMemberModel model
    ) => new(
        client,
        context.Path.RequireIdentity(T<GuildIdentity>()),
        context.Path.RequireIdentity(T<ThreadChannelIdentity>()),
        model,
        context.Path.GetIdentity(T<MemberIdentity>()),
        context.Path.GetIdentity(T<UserIdentity>()),
        context.TryGetActor<GatewayThreadMemberActor>()
    );

    public override ValueTask UpdateAsync(
        IThreadMemberModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        Model = model;

        return ValueTask.CompletedTask;
    }

    public override IThreadMemberModel GetModel() => Model;
}
