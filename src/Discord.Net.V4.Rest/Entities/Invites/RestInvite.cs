using Discord.Models;
using Discord.Rest;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestInviteActor :
    RestActor<string, RestInvite, InviteIdentity>,
    IInviteActor
{
    internal override InviteIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(invite))]
    public RestInviteActor(
        DiscordRestClient client,
        InviteIdentity invite
    ) : base(client, invite)
    {
        Identity = invite | this;
    }

    [SourceOfTruth]
    internal virtual RestInvite CreateEntity(IInviteModel model)
        => RestInvite.Construct(Client, model);
}

public partial class RestInvite :
    RestEntity<string>,
    IInvite,
    IConstructable<RestInvite, IInviteModel, DiscordRestClient>,
    IContextConstructable<RestInvite, IInviteModel, RestInvite.Context, DiscordRestClient>
{
    public readonly record struct Context(
        GuildIdentity? Guild = null,
        GuildChannelIdentity? Channel = null,
        GuildScheduledEventIdentity? ScheduledEvent = null
    );

    public InviteType Type => (InviteType)Model.Type;

    [SourceOfTruth] public RestUserActor? Inviter { get; private set; }

    public InviteTargetType? TargetType => (InviteTargetType?)Model.TargetType;

    [SourceOfTruth] public RestUserActor? TargetUser { get; private set; }

    public int? ApproximatePresenceCount => Model.ApproximatePresenceCount;

    public int? ApproximateMemberCount => Model.ApproximateMemberCount;

    public DateTimeOffset? ExpiresAt => Model.ExpiresAt;

    [ProxyInterface(typeof(IInviteActor))] internal virtual RestInviteActor Actor { get; }

    internal IInviteModel Model { get; private set; }

    public RestInvite(
        DiscordRestClient client,
        IInviteModel model,
        RestInviteActor? actor = null,
        GuildIdentity? guild = null,
        GuildChannelIdentity? channel = null
    ) : base(client, model.Id)
    {
        Model = model;

        Actor = actor ?? new(client, InviteIdentity.Of(this));

        Inviter = model.InviterId.Map(
            static (id, client) => RestUserActor.Factory(client, UserIdentity.Of(id)),
            client
        );

        TargetUser = model.TargetUserId.Map(
            static (id, client) => RestUserActor.Factory(client, UserIdentity.Of(id)),
            client
        );
    }

    public static RestInvite Construct(DiscordRestClient client, IInviteModel model)
        => Construct(client, default, model);

    public static RestInvite Construct(DiscordRestClient client, Context context, IInviteModel model)
    {
        if(context.Guild is not null || model.GuildId.HasValue)
            return RestGuildInvite.Construct(
                client,
                new RestGuildInvite.Context(
                    context.Guild | model.GuildId!.Value,
                    context.Channel,
                    context.ScheduledEvent
                ),
                model
            );

        return new(client, model, guild: context.Guild, channel: context.Channel);
    }

    public ValueTask UpdateAsync(IInviteModel model, CancellationToken token = default)
    {
        Inviter = Inviter.UpdateFrom(
            model.InviterId,
            RestUserActor.Factory,
            Client
        );

        TargetUser = TargetUser.UpdateFrom(
            model.TargetUserId,
            RestUserActor.Factory,
            Client
        );

        Model = model;

        return ValueTask.CompletedTask;
    }

    public IInviteModel GetModel() => Model;
}
