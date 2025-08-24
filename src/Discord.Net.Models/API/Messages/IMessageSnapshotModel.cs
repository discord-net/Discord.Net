using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IMessageSnapshotModel : IModel
{
    // TODO: this is partial
    IMessageModel Message { get; }
}