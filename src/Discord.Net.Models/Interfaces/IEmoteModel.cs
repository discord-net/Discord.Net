using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models;

[ModelEquality]
[HasPartialVariant]
public partial interface IEmoteModel : IModel
{
    [NullableInPartial]
    string Name { get; }
}
