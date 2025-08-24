using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IReactionModel : IModel
{
    int Count { get; }
    
    IReactionCountDetailsModel CountDetails { get; }
    
    bool Me { get; }
    
    bool MeBurst { get; }
    
    EmojiId Emoji { get; }
    
    IReadOnlyList<Color> BurstColors { get; }
}