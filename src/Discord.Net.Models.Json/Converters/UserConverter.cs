using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json.Converters;

public sealed class UserConverter : JsonConverter<IUserModel>
{
    private readonly JsonTypeInfo<UserModel> _userTypeInfo;
    private readonly JsonTypeInfo<CurrentUserModel> _currentUserTypeInfo;

    public UserConverter(
        JsonTypeInfo<UserModel> userTypeInfo,
        JsonTypeInfo<CurrentUserModel> currentUserTypeInfo
    )
    {
        _userTypeInfo = userTypeInfo;
        _currentUserTypeInfo = currentUserTypeInfo;
    }

    public override IUserModel? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        // if the email property is specified, regardless of its value, we'll deserialize as a current 
        // user model
        if (JsonNode.Parse(ref reader) is not JsonObject jsonObject)
            throw new JsonException("Expected object type");
        
        return jsonObject.TryGetPropertyValue("email", out _)
            ? jsonObject.Deserialize(_currentUserTypeInfo)
            : jsonObject.Deserialize(_userTypeInfo);
    }

    public override void Write(Utf8JsonWriter writer, IUserModel value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value is ICurrentUserModel ? _currentUserTypeInfo : _userTypeInfo);
    }
}