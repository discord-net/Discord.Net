using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class PollAnswerVoters : IModelSourceOfMultiple<IUserModel>
{
    [JsonPropertyName("users")]
    public required User[] Users { get; set; }

    IEnumerable<IUserModel> IModelSourceOfMultiple<IUserModel>.GetModels() => Users;
}
