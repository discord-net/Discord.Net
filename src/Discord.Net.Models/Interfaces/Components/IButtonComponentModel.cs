using Discord.Models.Json;

namespace Discord.Models;

public interface IButtonComponentModel : IMessageComponentModel
{
    int Style { get; }

    string? Label { get; }

    IEmoteModel? Emote { get; }

    string? CustomId { get; }

    string? Url { get; }

    bool? IsDisabled { get; }
}
