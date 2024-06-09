using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildMedia)]
public sealed class GuildMediaChannel : GuildChannelBase
{
}
