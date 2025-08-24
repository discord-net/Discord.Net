using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IMessagePinModel : IModel
{
    IMessageModel Message { get; }
    DateTimeOffset PinnedAt { get; }
}