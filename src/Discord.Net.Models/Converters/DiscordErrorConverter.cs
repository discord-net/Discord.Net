using Discord.Models.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ErrorMap = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Discord.Models.Json.Error>>;

namespace Discord.Converters;

public sealed class DiscordErrorConverter : JsonConverter<DiscordError>
{
    public static readonly DiscordErrorConverter Instance = new();

    public override DiscordError? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var obj = JsonDocument.ParseValue(ref reader).RootElement;

        if (obj.ValueKind is not JsonValueKind.Object)
            return null;

        var map = new ErrorMap();

        ProcessNode(obj.GetProperty("errors"), [], map, options);

        return new DiscordError()
        {
            Errors = map,
            Message = obj.GetProperty("message").GetString()!,
            Code = obj.GetProperty("code").GetInt32()
        };
    }

    private void ProcessNode(JsonElement node, string[] path, ErrorMap errors, JsonSerializerOptions options)
    {
        foreach (var property in node.EnumerateObject())
        {
            if (property.Name == "_errors")
            {
                foreach (var error in property.Value.EnumerateArray())
                {
                    AddError(error, path, errors, options);
                }

                return;
            }

            ProcessNode(property.Value, [..path, property.Name], errors, options);
        }
    }

    private static void AddError(JsonElement error, string[] pathParts, ErrorMap errorMap, JsonSerializerOptions options)
    {
        var errorModel = error.Deserialize<Error>(options)!;
        var path = ToJsonPath(pathParts);

        if (!errorMap.TryGetValue(path, out var errors))
            errorMap[path] = [errorModel];
        else errors.Add(errorModel);
    }

    private static string ToJsonPath(string[] path)
    {
        var sb = new StringBuilder("$");

        foreach (var part in path)
        {
            if (int.TryParse(part, out _))
                sb.Append($".[{part}]");
            else
                sb.Append($".{part}");
        }

        return sb.ToString();
    }

    public override void Write(Utf8JsonWriter writer, DiscordError value, JsonSerializerOptions options)
        => throw new NotSupportedException();
}
