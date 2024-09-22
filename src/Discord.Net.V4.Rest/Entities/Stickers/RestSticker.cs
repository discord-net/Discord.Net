using Discord.Models;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestStickerActor :
    RestActor<RestStickerActor, ulong, RestSticker, IStickerModel>,
    IStickerActor
{
    internal override StickerIdentity Identity { get; }

    [TypeFactory]
    public RestStickerActor(
        DiscordRestClient client,
        StickerIdentity sticker
    ) : base(client, sticker)
    {
        Identity = sticker | this;
    }

    [SourceOfTruth]
    internal override RestSticker CreateEntity(IStickerModel model)
        => RestSticker.Construct(Client, this, model);
}

public partial class RestSticker :
    RestEntity<ulong>,
    ISticker,
    IRestConstructable<RestSticker, RestStickerActor, IStickerModel>
{
    public string Name => Model.Name;

    public StickerFormatType Format => (StickerFormatType) Model.FormatType;

    [SourceOfTruth] public RestStickerPackActor? Pack { get; }

    public string? Description => Model.Description;

    public IReadOnlyCollection<string> Tags { get; private set; }

    public StickerType Type => (StickerType) Model.Type;

    public int? SortOrder => Model.SortValue;

    [ProxyInterface] internal virtual RestStickerActor Actor { get; }

    internal virtual IStickerModel Model => _model;

    private IStickerModel _model;

    internal RestSticker(
        DiscordRestClient client,
        IStickerModel model,
        RestStickerActor actor
    ) : base(client, model.Id)
    {
        _model = model;
        Tags = model.Tags.Split(',').ToImmutableArray();

        Actor = actor;

        Pack = model.PackId.HasValue
            ? client.StickerPacks[model.PackId.Value]
            : null;
    }

    public static RestSticker Construct(DiscordRestClient client, RestStickerActor actor, IStickerModel model)
    {
        return model switch
        {
            IGuildStickerModel guildSticker => RestGuildSticker.Construct(
                client,
                actor as RestGuildStickerActor ?? client.Guilds[guildSticker.GuildId].Stickers[model.Id],
                guildSticker
            ),
            _ => new RestSticker(client, model, actor)
        };
    }

    public virtual ValueTask UpdateAsync(IStickerModel model, CancellationToken token = default)
    {
        if (_model.Tags != model.Tags)
            Tags = Model.Tags.Split(',').ToImmutableArray();

        _model = model;

        return ValueTask.CompletedTask;
    }

    public virtual IStickerModel GetModel() => Model;
}