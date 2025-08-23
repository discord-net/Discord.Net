using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IModifyCurrentUserParams : IParametersModel
{
    Optional<string> Username { get; }
    Optional<ImageData?> Avatar { get; }
    Optional<ImageData?> Banner { get; }
}