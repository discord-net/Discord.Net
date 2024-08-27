using Discord.Models;
using Discord.Rest;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestInviteActor :
    RestActor<string, RestInvite, InviteIdentity, IInviteModel>,
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
    internal override RestInvite CreateEntity(IInviteModel model)
        => RestInvite.Construct(Client, this, model);
}

public partial class RestInvite :
    RestEntity<string>,
    IInvite,
    IRestConstructable<RestInvite, RestInviteActor, IInviteModel>
{
    public InviteType Type => (InviteType) Model.Type;

    [SourceOfTruth] public RestUserActor? Inviter { get; private set; }

    public InviteTargetType? TargetType => (InviteTargetType?) Model.TargetType;

    [SourceOfTruth] public RestUserActor? TargetUser { get; private set; }

    public int? ApproximatePresenceCount => Model.ApproximatePresenceCount;

    public int? ApproximateMemberCount => Model.ApproximateMemberCount;

    public DateTimeOffset? ExpiresAt => Model.ExpiresAt;

    [ProxyInterface(typeof(IInviteActor))] internal virtual RestInviteActor Actor { get; }

    internal IInviteModel Model { get; private set; }

    public RestInvite(
        DiscordRestClient client,
        IInviteModel model,
        RestInviteActor actor
    ) : base(client, model.Id)
    {
        Model = model;

        Actor = actor;

        Inviter = model.InviterId.Map(
            static (id, client) => RestUserActor.Factory(client, UserIdentity.Of(id)),
            client
        );

        TargetUser = model.TargetUserId.Map(
            static (id, client) => RestUserActor.Factory(client, UserIdentity.Of(id)),
            client
        );
    }

    public static RestInvite Construct(DiscordRestClient client, RestInviteActor actor, IInviteModel model)
    {
        if (model is {GuildId: not null, ChannelId: not null} guildChannelModel)
        {
            return RestGuildChannelInvite.Construct(
                client,
                actor as RestGuildChannelInviteActor ?? new RestGuildChannelInviteActor(client,
                    GuildIdentity.Of(guildChannelModel.GuildId.Value),
                    GuildChannelIdentity.Of(guildChannelModel.ChannelId.Value),
                    GuildChannelInviteIdentity.Of(model.Id)
                ),
                model
            );
        }

        if (model.ChannelId.HasValue)
        {
            return RestChannelInvite.Construct(
                client,
                actor as RestChannelInviteActor ?? new RestChannelInviteActor(client,
                    ChannelIdentity.Of(model.ChannelId.Value),
                    ChannelInviteIdentity.Of(model.Id)
                ),
                model
            );
        }

        if (model.GuildId.HasValue)
        {
            return RestGuildInvite.Construct(
                client,
                actor as RestGuildInviteActor ?? new RestGuildInviteActor(client,
                    GuildIdentity.Of(model.GuildId.Value),
                    GuildInviteIdentity.Of(model.Id)
                ),
                model
            );
        }
        
        return new(client, model, actor);
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