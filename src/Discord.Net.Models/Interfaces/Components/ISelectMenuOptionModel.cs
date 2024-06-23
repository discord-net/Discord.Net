namespace Discord.Models;

public interface ISelectMenuOptionModel
{
    string Label { get; }
    string Value { get; }
    string? Description { get; }
    IEmoteModel? Emote { get; }
    bool? IsDefault { get; }
}
