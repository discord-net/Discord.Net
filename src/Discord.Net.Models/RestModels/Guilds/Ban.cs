using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Ban : IBanModel, IModelSource, IModelSourceOf<IUserModel>
{
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }

    public IEnumerable<IEntityModel> GetDefinedModels() => [User];
    ulong IBanModel.UserId => User.Id;

    IUserModel IModelSourceOf<IUserModel>.Model => User;
}
