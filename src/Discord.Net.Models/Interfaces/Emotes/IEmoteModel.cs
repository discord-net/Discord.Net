using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models;

[ModelEquality]
[HasPartialVariant]
public partial interface IEmoteModel : IEntityModel<DiscordEmojiId>
{
    [NullableInPartial]
    string Name { get; }
}
