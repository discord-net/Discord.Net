using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IPollMediaModel : IModel
{
    Optional<string> Text { get; }
    Optional<EmojiId> Emoji { get; }
}