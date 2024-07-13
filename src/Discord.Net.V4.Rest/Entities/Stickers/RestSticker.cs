using Discord.Models;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Discord.Rest.Stickers;

public partial class RestSticker :
    RestEntity<ulong>,
    ISticker,
    IConstructable<RestSticker, IStickerModel, DiscordRestClient>
{
    public string Name => Model.Name;

    public StickerFormatType Format => (StickerFormatType)Model.FormatType;

    public ulong? PackId => Model.PackId;

    public string? Description => Model.Description;

    public IReadOnlyCollection<string> Tags { get; private set; }

    public StickerType Type => (StickerType)Model.Type;

    public int? SortOrder => Model.SortValue;

    internal virtual IStickerModel Model => _model;

    private IStickerModel _model;

    internal RestSticker(DiscordRestClient client, IStickerModel model) : base(client, model.Id)
    {
        _model = model;
        Tags = model.Tags.Split(',').ToImmutableArray();
    }

    public static RestSticker Construct(DiscordRestClient client, IStickerModel model)
    {
        return model switch
        {
            IGuildStickerModel guildSticker
                => RestGuildSticker.Construct(client, guildSticker, GuildIdentity.Of(guildSticker.GuildId)),
            _ => new RestSticker(client, model)
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
