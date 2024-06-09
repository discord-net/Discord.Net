using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[JsonConverter(typeof(EmoteConverter))]
public interface IEmote : IEmojiModel
{
    bool IsGuildEmote => this is GuildEmote;
    bool IsEmoji => this is Emoji;
}
