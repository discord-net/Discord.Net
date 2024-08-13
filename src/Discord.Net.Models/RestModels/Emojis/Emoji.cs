using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(GuildEmote.Id), WhenSpecified = false)]
public sealed class Emoji : Emote, IEmojiModel;