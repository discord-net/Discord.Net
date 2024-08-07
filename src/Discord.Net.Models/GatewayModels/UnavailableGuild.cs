using Discord.Models.Json;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Unavailable), true)]
public sealed class UnavailableGuild : GuildCreated, IUnavailableGuild;
