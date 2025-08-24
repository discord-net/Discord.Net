using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ComponentType.TextDisplay)]
public interface ITextDisplayComponentModel :
    ISectionComponentAtom,
    IContainerAtom
{
    string Content { get; }
}