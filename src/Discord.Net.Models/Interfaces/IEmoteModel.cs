using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models;

[ModelEquality]
[JsonConverter(typeof(EmoteConverter))]
public partial interface IEmoteModel : IEntityModel
{
    string? Name { get; }
}
