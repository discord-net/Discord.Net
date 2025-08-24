using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IMessageComponentModel : IModel
{
    ComponentType Type { get; }
    int? Id { get; }
}