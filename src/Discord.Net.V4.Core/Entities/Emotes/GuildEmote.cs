using Discord.Models;
using System.Collections.Immutable;

namespace Discord;

public sealed class GuildEmote :
    IEntity<ulong>,
    IEmote,
    IEntityProperties<Models.Json.GuildEmote>,
    IConstructable<GuildEmote, IGuildEmoteModel>
{
    public GuildEmote(IDiscordClient client, IGuildEmoteModel model)
    {
        Client = client;
        Id = model.Id;
        Name = model.Name!;
        RoleIds = model.Roles.ToImmutableArray();
        Creator = model.UserId.HasValue
            ? client.User(model.UserId.Value)
            : null;
        RequiresColons = model.RequireColons;
        IsManaged = model.IsManaged;
        IsAnimated = model.IsAnimated;
        IsAvailable = model.IsAvailable;
    }

    public IReadOnlyCollection<ulong> RoleIds { get; }
    public ILoadableUserActor? Creator { get; }
    public bool RequiresColons { get; }
    public bool IsManaged { get; }
    public bool IsAnimated { get; }
    public bool IsAvailable { get; }

    public static GuildEmote Construct(IDiscordClient client, IGuildEmoteModel model)
        => new(client, model);

    public string Name { get; }


    public IEmoteModel ToApiModel(IEmoteModel? existing = default)
        => ToApiModel(null);

    public ulong Id { get; }

    public IDiscordClient Client { get; }

    public Models.Json.GuildEmote ToApiModel(Models.Json.GuildEmote? existing = default)
    {
        existing ??= new Models.Json.GuildEmote {Id = Id};

        existing.Name = Name;
        existing.RoleIds = RoleIds.ToArray();
        existing.RequireColons = RequiresColons;
        existing.Managed = IsManaged;
        existing.Animated = IsAnimated;
        existing.Available = IsAvailable;

        return existing;
    }
}
