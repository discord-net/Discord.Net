using Discord.Models;
using Discord.Models.Json;
using Discord.Models.Json.Stickers;
using Discord.Rest.Guilds;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Rest.Stickers;

public partial class RestLoadableGuildStickerActor(
    DiscordRestClient client,
    GuildIdentity guild,
    GuildStickerIdentity sticker
):
    RestGuildStickerActor(client, guild, sticker),
    ILoadableGuildStickerActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuildSticker>))]
    internal RestLoadable<ulong, RestGuildSticker, IGuildSticker, IGuildStickerModel> Loadable { get; }
        = RestLoadable<ulong, RestGuildSticker, IGuildSticker, IGuildStickerModel>
            .FromContextConstructable<RestGuildSticker, GuildIdentity>(
                client,
                sticker,
                (guild, id) => Routes.GetGuildSticker(guild.Id, id),
                guild
            );
}

[ExtendInterfaceDefaults(
    typeof(IGuildStickerActor),
    typeof(IModifiable<ulong, IGuildStickerActor, ModifyStickerProperties, ModifyGuildStickersParams>),
    typeof(IDeletable<ulong, IGuildStickerActor>)
)]
public partial class RestGuildStickerActor(
    DiscordRestClient client,
    GuildIdentity guild,
    GuildStickerIdentity sticker
) :
    RestActor<ulong, RestGuildSticker, GuildStickerIdentity>(client, sticker),
    IGuildStickerActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guild);

    ILoadableGuildActor IGuildRelationship.Guild => Guild;

    IGuildSticker IEntityProvider<IGuildSticker, IGuildStickerModel>.CreateEntity(IGuildStickerModel model)
        => RestGuildSticker.Construct(Client, model, Guild.Identity);
}

public sealed partial class RestGuildSticker :
    RestSticker,
    IGuildSticker,
    IContextConstructable<RestGuildSticker, IGuildStickerModel, GuildIdentity, DiscordRestClient>
{
    public RestLoadableGuildMemberActor? Author { get; private set; }

    public bool? IsAvailable => Model.Available;

    internal override IGuildStickerModel Model => _model;

    [ProxyInterface(
        typeof(IGuildStickerActor),
        typeof(IGuildRelationship),
        typeof(IEntityProvider<IGuildSticker, IGuildStickerModel>)
    )]
    internal RestGuildStickerActor Actor { get; }

    private IGuildStickerModel _model;

    internal RestGuildSticker(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildStickerModel model,
        RestGuildStickerActor? actor = null
    ) : base(client, model)
    {
        _model = model;

        Actor = actor ?? new(client, guild, GuildStickerIdentity.Of(this));

        Author = ConstructAuthorActor(client, guild, model, model.AuthorId);
    }

    public static RestGuildSticker Construct(DiscordRestClient client, IGuildStickerModel model, GuildIdentity guild)
        => new(client, guild, model);

    public ValueTask UpdateAsync(IGuildStickerModel model, CancellationToken token = default)
    {
        if (_model.AuthorId != model.AuthorId)
        {
            if (model.AuthorId is not null)
            {
                Author ??= ConstructAuthorActor(Client, Actor.Guild.Identity, model, model.AuthorId);

                Author.Loadable.Id = model.AuthorId.Value;
            }
            else
            {
                Author = null;
            }
        }

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildStickerModel GetModel() => Model;


    [return: NotNullIfNotNull(nameof(authorId))]
    private static RestLoadableGuildMemberActor? ConstructAuthorActor(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildStickerModel model,
        ulong? authorId)
    {
        var userAuthorIdentity = authorId.Map(
            static (id, model, client) =>
                UserIdentity.OfNullable(
                    model.GetReferencedEntityModel<ulong, IUserModel>(id),
                    model => RestUser.Construct(client, model)
                ),
            model,
            client
        );

        return authorId.Map(static (id, client, guild, userIdentity) => new RestLoadableGuildMemberActor(
            client,
            guild,
            MemberIdentity.Of(id),
            userIdentity
        ), client, guild, userAuthorIdentity);
    }

    ILoadableGuildMemberActor? IGuildSticker.Author => Author;
}
