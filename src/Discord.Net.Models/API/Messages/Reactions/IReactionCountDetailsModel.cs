using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IReactionCountDetailsModel : IModel
{
    int Burst { get; }
    int Normal { get; }
}