using Discord.Entities.Channels.Threads;
using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Discord.Rest.Extensions;
using System.Collections.Immutable;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestThreadChannelActor :
    RestChannelActor,
    IThreadChannelActor,
    IRestActor<RestThreadChannelActor, ulong, RestThreadChannel, IThreadChannelModel>,
    IRestMessageChannelTrait
{
    [SourceOfTruth]
    public RestThreadMemberActor
        .Enumerable
        .Indexable
        .WithCurrentMember
        .WithPagedVariant
        .BackLink<RestThreadChannelActor>
        Members { get; }

    [SourceOfTruth] internal override ThreadIdentity Identity { get; }

    [TypeFactory]
    public RestThreadChannelActor(
        DiscordRestClient client,
        ThreadIdentity thread,
        ThreadMemberIdentity? currentThreadMember = null,
        IActorProvider<RestThreadMemberActor, ulong>? memberProvider = null
    ) : base(client, thread)
    {
        thread = Identity = thread | this;

        memberProvider ??= RestActorProvider.GetOrCreate(
            client,
            Template.Of<ThreadMemberIdentity>(),
            thread
        );

        Members = new(
            this,
            client,
            memberProvider,
            Routes
                .ListThreadMembers(thread.Id)
                .AsRequiredProvider(),
            currentThreadMember?.Actor ?? new(
                client,
                thread,
                currentThreadMember ?? ThreadMemberIdentity.Of(client.Users.Current.Id)
            ),
            new(
                client,
                memberProvider,
                new RestPagingProvider<
                    IThreadMemberModel,
                    PageThreadMembersParams,
                    RestThreadMember
                >(
                    client,
                    (model, _) => Members![model.Id].CreateEntity(model),
                    this
                )
            )
        );
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal virtual RestThreadChannel CreateEntity(IThreadChannelModel model)
        => RestThreadChannel.Construct(Client, this, model);
}

public sealed partial class RestGuildThreadChannelActor :
    RestGuildChannelActor,
    IGuildThreadChannelActor,
    IRestActor<RestGuildThreadChannelActor, ulong, RestThreadChannel, IThreadChannelModel>,
    IRestMessageChannelTrait
{
    [SourceOfTruth]
    public RestGuildThreadMemberActor
        .Enumerable
        .Indexable
        .WithCurrentMember
        .WithPagedVariant
        .BackLink<RestGuildThreadChannelActor>
        Members { get; }
    
    [SourceOfTruth] internal override GuildThreadIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(thread))]
    public RestGuildThreadChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildThreadIdentity thread,
        GuildThreadMemberIdentity? currentThreadMember = null
    ) : base(client, guild, thread)
    {
        Identity = thread;

        Members = new(
            this,
            client,
            RestActorProvider.GetOrCreate(
                client,
                Template.Of<GuildThreadMemberIdentity>(),
                guild,
                thread
            ),
            Routes
                .ListThreadMembers(thread.Id)
                .AsRequiredProvider(),
            currentThreadMember?.Actor ?? new(
                client,
                guild,
                thread,
                currentThreadMember ?? GuildThreadMemberIdentity.Of(client.Users.Current.Id)
            ),
            new(
                client,
                RestActorProvider.GetOrCreate(
                    client,
                    Template.Of<GuildThreadMemberIdentity>(),
                    guild,
                    thread
                ),
                new RestPagingProvider<
                    IThreadMemberModel,
                    PageThreadMembersParams,
                    RestThreadMember
                >(
                    client,
                    (model, _) => Members![model.Id].CreateEntity(model),
                    this
                )
            )
        );
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal RestThreadChannel CreateEntity(IThreadChannelModel model)
        => RestThreadChannel.Construct(Client, this, model);
}

public partial class RestThreadChannel :
    RestGuildChannel,
    IThreadChannel,
    IRestConstructable<RestThreadChannel, RestGuildThreadChannelActor, IThreadChannelModel>,
    IRestConstructable<RestThreadChannel, RestThreadChannelActor, IThreadChannelModel>
{
    [SourceOfTruth] public RestThreadableChannelActor Parent { get; }

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

    [ProxyInterface(typeof(IGuildThreadChannelActor))]
    internal override RestGuildThreadChannelActor Actor { get; }

    internal override IThreadChannelModel Model => _model;

    private IThreadChannelModel _model;

    internal RestThreadChannel(
        DiscordRestClient client,
        IThreadChannelModel model,
        RestGuildThreadChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;

        Actor = actor;
        Parent = Guild.Channels.Threadable[model.ParentId];
        Creator = client.Users[model.OwnerId];

        AppliedTags = model.AppliedTags.ToImmutableArray();
    }


    public static RestThreadChannel Construct(
        DiscordRestClient client,
        RestThreadChannelActor actor,
        IThreadChannelModel model
    ) => new(
        client,
        model,
        client.Guilds[model.GuildId].Threads[model.Id]
    );
    
    public static RestThreadChannel Construct(
        DiscordRestClient client,
        RestGuildThreadChannelActor actor,
        IThreadChannelModel model
    ) => new(client, model, actor);

    [CovariantOverride]
    public ValueTask UpdateAsync(IThreadChannelModel model, CancellationToken token = default)
    {
        if (!AppliedTags.SequenceEqual(model.AppliedTags))
            AppliedTags = model.AppliedTags.ToImmutableArray();

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IThreadChannelModel GetModel() => Model;
}