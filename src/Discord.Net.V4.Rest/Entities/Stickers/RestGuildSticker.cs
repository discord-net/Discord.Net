using Discord.Models;
using Discord.Models.Json;
using Discord.Models.Json.Stickers;
using Discord.Rest.Guilds;

namespace Discord.Rest.Stickers;

public partial class RestLoadableGuildStickerActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestGuildStickerActor(client, guildId, id),
    ILoadableGuildStickerActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuildSticker>))]
    internal RestLoadable<ulong, RestGuildSticker, IGuildSticker, Sticker> Loadable { get; }
        = RestLoadable<ulong, RestGuildSticker, IGuildSticker, Sticker>
            .FromContextConstructable<RestGuildSticker, ulong>(
                client,
                id,
                Routes.GetGuildSticker,
                guildId
            );
}

[ExtendInterfaceDefaults(
    typeof(IGuildStickerActor),
    typeof(IModifiable<ulong, IGuildStickerActor, ModifyStickerProperties, ModifyGuildStickersParams>),
    typeof(IDeletable<ulong, IGuildStickerActor>)
)]
public partial class RestGuildStickerActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestActor<ulong, RestGuildSticker>(client, id),
    IGuildStickerActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guildId);

    ILoadableGuildActor IGuildRelationship.Guild => Guild;
}

public sealed partial class RestGuildSticker(DiscordRestClient client, ulong guildId, IStickerModel model, RestGuildStickerActor? actor = null) :
    RestSticker(client, model),
    IGuildSticker,
    IContextConstructable<RestGuildSticker, IStickerModel, ulong, DiscordRestClient>
{
    [ProxyInterface(typeof(IGuildStickerActor), typeof(IGuildRelationship))]
    internal RestGuildStickerActor Actor { get; } = actor ?? new(client, guildId, model.Id);

    public static RestGuildSticker Construct(DiscordRestClient client, IStickerModel model, ulong context)
        => new(client, context, model);

    protected override void OnModelUpdated()
    {
        base.OnModelUpdated();

        if (Model.AuthorId != Author?.Id)
        {
            if (Model.AuthorId.HasValue)
                (Author ??= new(Client, guildId, Model.AuthorId.Value)).Loadable.Id = Model.AuthorId.Value;
            else
                Author = null;
        }
    }

    public RestLoadableGuildMemberActor? Author { get; private set; }
        = model.AuthorId.HasValue
            ? new(client, guildId, model.AuthorId.Value)
            : null;

    ILoadableGuildMemberActor? IGuildSticker.Author => Author;
}
