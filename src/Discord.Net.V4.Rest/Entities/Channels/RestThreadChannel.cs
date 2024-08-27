using Discord.Entities.Channels.Threads;
using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Discord.Rest.Extensions;
using System.Collections.Immutable;

namespace Discord.Rest;

using MessageChannelTrait = RestMessageChannelTrait<RestThreadChannelActor, ThreadIdentity>;

[ExtendInterfaceDefaults]
public partial class RestThreadChannelActor :
    RestChannelActor,
    IThreadChannelActor,
    IRestActor<ulong, RestThreadChannel, ThreadIdentity, IThreadChannelModel>
{
    [ProxyInterface(typeof(IMessageChannelTrait))]
    internal MessageChannelTrait MessageChannelTrait { get; }

    [SourceOfTruth] public virtual RestThreadMemberActor CurrentThreadMember { get; }

    [SourceOfTruth] public virtual ThreadMemberLink.Enumerable.Indexable ThreadMembers { get; }

    [SourceOfTruth] internal override ThreadIdentity Identity { get; }

    [TypeFactory]
    public RestThreadChannelActor(
        DiscordRestClient client,
        ThreadIdentity thread,
        ThreadMemberIdentity? currentThreadMember = null
    ) : base(client, thread)
    {
        thread = Identity = thread | this;

        MessageChannelTrait = new(client, this, thread);

        CurrentThreadMember =
            currentThreadMember?.Actor
            ??
            new RestThreadMemberActor(
                client,
                thread,
                currentThreadMember ?? ThreadMemberIdentity.Of(client.CurrentUser.Id),
                client.CurrentUser.Identity
            );

        ThreadMembers = RestActors.Fetchable(
            Template.T<RestThreadMemberActor>(),
            client,
            RestThreadMemberActor.Factory,
            Guild.Identity,
            thread,
            entityFactory: RestThreadMember.Construct,
            new RestThreadMember.Context(Guild.Identity, thread),
            IThreadMember.FetchManyRoute(this)
        );
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal virtual RestThreadChannel CreateEntity(IThreadChannelModel model)
        => RestThreadChannel.Construct(Client, new RestThreadChannel.Context(
            Guild.Identity,
            CurrentThreadMember.Identity
        ), model);
}

public sealed partial class RestGuildThreadChannelActor :
    RestThreadChannelActor,
    IGuildThreadChannelActor,
    IRestActor<ulong, RestThreadChannel, GuildThreadIdentity, IThreadChannelModel>
{
    [SourceOfTruth]
    public override RestGuildThreadMemberActor CurrentThreadMember { get; }
    
    [SourceOfTruth]
    public override GuildThreadMemberLink.Enumerable.Indexable.BackLink<RestGuildThreadChannelActor> ThreadMembers { get; }
    
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth] internal override GuildThreadIdentity Identity { get; }

    public RestGuildThreadChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildThreadIdentity thread,
        ThreadMemberIdentity? currentThreadMember = null
    ) : base(client, thread)
    {
        Identity = thread;
        Guild = guild.Actor ?? client.Guilds[guild.Id];
    }

    internal void UpdateCurrentMember(IThreadMemberModel model)
    {
        CurrentThreadMember.Identity
    }

    [SourceOfTruth]
    internal override RestThreadChannel CreateEntity(IThreadChannelModel model)
        => RestThreadChannel.Construct(Client, this, model);
}

public partial class RestThreadChannel :
    RestChannel,
    IThreadChannel,
    IRestConstructable<RestThreadChannel, RestThreadChannelActor, IThreadChannelModel>
{
    [SourceOfTruth] public RestThreadableChannelActor Parent { get; }

    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth] public RestUserActor Creator { get; }

    public new ThreadType Type => (ThreadType) Model.Type;

    public bool HasJoined => Model.HasJoined;

    public bool IsArchived => Model.IsArchived;

    public ThreadArchiveDuration AutoArchiveDuration => (ThreadArchiveDuration) Model.AutoArchiveDuration;

    public DateTimeOffset ArchiveTimestamp => Model.ArchiveTimestamp;

    public bool IsLocked => Model.IsLocked;

    public int MemberCount => Model.MemberCount;

    public int MessageCount => Model.MessageCount;

    public bool? IsInvitable => Model.IsInvitable;

    public IReadOnlyCollection<ulong> AppliedTags { get; private set; }

    public DateTimeOffset CreatedAt => Model.CreatedAt ?? SnowflakeUtils.FromSnowflake(Id);

    [ProxyInterface(typeof(IThreadChannelActor))]
    internal override RestThreadChannelActor Actor { get; }

    internal override IThreadChannelModel Model => _model;

    private IThreadChannelModel _model;

    internal RestThreadChannel(
        DiscordRestClient client,
        RestGuildActor guild,
        RestThreadableChannelActor parent,
        IThreadChannelModel model,
        RestThreadChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;

        Actor = actor;
        Guild = guild;
        Parent = parent;

        AppliedTags = model.AppliedTags.ToImmutableArray();
    }


    public static RestThreadChannel Construct(
        DiscordRestClient client,
        RestThreadChannelActor actor,
        IThreadChannelModel model)
    {
        var guild = actor is RestGuildThreadChannelActor guildThreadActor
            ? guildThreadActor.Guild
            : client.Guilds[model.GuildId];

        var parent = guild.Channels.Threadable[model.ParentId];

        return new RestThreadChannel(client, guild, parent, model, actor);
    }

    [CovariantOverride]
    public ValueTask UpdateAsync(IThreadChannelModel model, CancellationToken token = default)
    {
        if (!AppliedTags.SequenceEqual(model.AppliedTags))
            AppliedTags = model.AppliedTags.ToImmutableArray();

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IThreadChannelModel GetModel() => Model;

    internal static ThreadMemberIdentity GetCurrentThreadMemberIdentity(
        DiscordRestClient client,
        GuildIdentity guild,
        ThreadIdentity thread,
        IThreadChannelModel? model)
    {
        return ThreadMemberIdentity.OfNullable(
            model?.GetReferencedEntityModel<ulong, IThreadMemberModel>(client.CurrentUser.Id),
            model => RestThreadMember.Construct(client, new RestThreadMember.Context(
                guild,
                thread
            ), model)
        ) ?? ThreadMemberIdentity.Of(client.CurrentUser.Id);
    }
}