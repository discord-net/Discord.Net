using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IModifiable = IModifiable<ulong, IGuildEmoteActor, EmoteProperties, ModifyEmojiParams, IGuildEmote, IGuildEmoteModel>;

public interface ILoadableGuildEmoteActor :
    IGuildEmoteActor,
    ILoadableEntity<ulong, IGuildEmote>;

public interface IGuildEmoteActor :
    IActor<ulong, IGuildEmote>,
    IGuildRelationship,
    IModifiable,
    IDeletable<ulong, IGuildEmoteActor>
{
    static IApiRoute IDeletable<ulong, IGuildEmoteActor>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteGuildEmoji(path.Require<IGuild>(), id);

    static IApiInOutRoute<ModifyEmojiParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyEmojiParams args
    ) => Routes.ModifyGuildEmoji(path.Require<IGuild>(), id, args);
}
