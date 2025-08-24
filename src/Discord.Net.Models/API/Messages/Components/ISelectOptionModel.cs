using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface ISelectOptionModel : IModel
{
    [Max(Constants.MAX_SELECT_OPTION_LABEL_LENGTH)]
    string Label { get; }
    
    [Max(Constants.MAX_SELECT_OPTION_VALUE_LENGTH)]
    string Value { get; }
    
    [Max(Constants.MAX_SELECT_OPTION_DESCRIPTION_LENGTH)]
    Optional<string> Description { get; }
    
    Optional<EmojiId> Emoji { get; }
    
    Optional<bool> Default { get; }
}