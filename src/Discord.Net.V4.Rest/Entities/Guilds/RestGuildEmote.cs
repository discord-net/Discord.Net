using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Guilds;

public sealed partial class RestLoadableGuildEmoteActor(
    DiscordRestClient client,
    GuildIdentity guild,
    GuildEmoteIdentity emote
) :
    RestGuildEmoteActor(client, guild, emote),
    ILoadableGuildEmoteActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuildEmote>))]
    internal RestLoadable<ulong, RestGuildEmote, IGuildEmote, IGuildEmoteModel> Loadable { get; } =
        RestLoadable<ulong, RestGuildEmote, IGuildEmote, IGuildEmoteModel>
            .FromContextConstructable<RestGuildEmote, GuildIdentity>(
                client,
                emote,
                (guild, id) => Routes.GetGuildEmoji(guild.Id, id),
                guild
            );
}

[ExtendInterfaceDefaults(typeof(IGuildEmoteActor))]
public partial class RestGuildEmoteActor(
    DiscordRestClient client,
    GuildIdentity guild,
    GuildEmoteIdentity emote
):
    RestActor<ulong, RestGuildEmote, GuildEmoteIdentity>(client, emote),
    IGuildEmoteActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guild);

    ILoadableGuildActor IGuildRelationship.Guild => Guild;

    IGuildEmote IEntityProvider<IGuildEmote, IGuildEmoteModel>.CreateEntity(IGuildEmoteModel model)
        => RestGuildEmote.Construct(Client, model, guild);
}

public sealed partial class RestGuildEmote :
    RestEntity<ulong>,
    IGuildEmote,
    IContextConstructable<RestGuildEmote, IGuildEmoteModel, GuildIdentity, DiscordRestClient>
{
    public IDefinedLoadableEntityEnumerable<ulong, IRole> Roles => throw new NotImplementedException();

    public RestLoadableUserActor? Creator { get; private set; }

    public string? Name => Model.Name;

    public bool IsManaged => Model.IsManaged;

    public bool RequireColons => Model.RequireColons;

    public bool IsAnimated => Model.IsAnimated;

    public bool IsAvailable => Model.IsAvailable;

    [ProxyInterface(
        typeof(IGuildEmoteActor),
        typeof(IGuildRelationship),
        typeof(IEntityProvider<IGuildEmote, IGuildEmoteModel>)
    )]
    internal RestGuildEmoteActor Actor { get; }

    internal IGuildEmoteModel Model { get; private set; }

    internal RestGuildEmote(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildEmoteModel model,
        RestGuildEmoteActor? actor = null
    ) : base(client, model.Id)
    {
        Actor = actor ?? new(client, guild, GuildEmoteIdentity.Of(this));
        Model = model;
    }

    public static RestGuildEmote Construct(DiscordRestClient client, IGuildEmoteModel model, GuildIdentity guild)
        => new(client, guild, model);

    public ValueTask UpdateAsync(IGuildEmoteModel model, CancellationToken token = default)
    {
        if (Model.UserId != model.UserId)
        {
            if (model.UserId is not null)
            {
                if (Creator is not null)
                    Creator.Loadable.Id = model.UserId.Value;
                else
                {
                    Creator = new RestLoadableUserActor(
                        Client,
                        UserIdentity.FromReferenced<RestUser, DiscordRestClient>(model, model.UserId.Value, Client)
                    );
                }
            }
            else
            {
                Creator = null;
            }
        }

        Model = model;

        return ValueTask.CompletedTask;
    }

    public IGuildEmoteModel GetModel() => Model;

    ILoadableEntity<ulong, IUser>? IGuildEmote.Creator => Creator;

}
