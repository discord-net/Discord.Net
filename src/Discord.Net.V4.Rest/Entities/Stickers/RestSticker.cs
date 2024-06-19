using Discord.Models;
using System.Collections.Immutable;

namespace Discord.Rest.Stickers;

public class RestSticker(DiscordRestClient client, IStickerModel model) :
    RestEntity<ulong>(client, model.Id),
    ISticker
{
    protected virtual IStickerModel Model { get; } = model;

    public string Name => Model.Name;

    public StickerFormatType Format => (StickerFormatType)Model.FormatType;

    public ulong? PackId => Model.PackId;

    public string? Description => Model.Description;

    public IReadOnlyCollection<string> Tags => Model.Tags.Split(',').ToImmutableArray();

    public StickerType Type => (StickerType)Model.Type;

    public bool? IsAvailable => Model.Available;

    public int? SortOrder => Model.SortValue;
}
