using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ComponentType.ActionRow)]
public interface IActionRowComponentModel : IContainerAtom
{
    IReadOnlyList<IMessageComponentModel> Components { get; }
}