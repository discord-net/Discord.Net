using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Guilds;

public sealed partial class RestLoadableGuildEmoteActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestGuildEmoteActor(client, guildId, id),
    ILoadableGuildEmoteActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuildEmote>))]
    internal RestLoadable<ulong, RestGuildEmote, IGuildEmote, GuildEmote> Loadable { get; } =
        RestLoadable<ulong, RestGuildEmote, IGuildEmote, GuildEmote>
            .FromContextConstructable<RestGuildEmote, ulong>(
                client,
                id,
                Routes.GetGuildEmoji,
                guildId
            );
}

[ExtendInterfaceDefaults(typeof(IGuildEmoteActor))]
public partial class RestGuildEmoteActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestActor<ulong, RestGuildEmote>(client, id),
    IGuildEmoteActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guildId);

    ILoadableGuildActor IGuildRelationship.Guild => Guild;
}

public sealed partial class RestGuildEmote(DiscordRestClient client, ulong guildId, IGuildEmoteModel model, RestGuildEmoteActor? actor = null) :
    RestEntity<ulong>(client, model.Id),
    IGuildEmote,
    IContextConstructable<RestGuildEmote, IGuildEmoteModel, ulong, DiscordRestClient>
{
    [ProxyInterface(typeof(IGuildEmoteActor), typeof(IGuildRelationship))]
    internal RestGuildEmoteActor Actor { get; } = actor ?? new(client, guildId, model.Id);

    internal IGuildEmoteModel Model { get; } = model;

    public static RestGuildEmote Construct(DiscordRestClient client, IGuildEmoteModel model, ulong context)
        => new(client, context, model);

    #region Loadables

    public IDefinedLoadableEntityEnumerable<ulong, IRole> Roles => throw new NotImplementedException();

    public RestLoadableUserActor? Creator
    {
        get
        {
            if (!Model.UserId.HasValue)
                return null;

            if (_creator is not null && _creator.Id == Model.UserId.Value) return _creator;

            _creator?.Loadable.Dispose();
            _creator = new(Client, Model.UserId.Value);

            return _creator;
        }
    }

    private RestLoadableUserActor? _creator;

    #endregion

    #region Properties

    public string? Name => Model.Name;

    public bool IsManaged => Model.IsManaged;

    public bool RequireColons => Model.RequireColons;

    public bool IsAnimated => Model.IsAnimated;

    public bool IsAvailable => Model.IsAvailable;

    #endregion

    ILoadableEntity<ulong, IUser>? IGuildEmote.Creator => Creator;
}
