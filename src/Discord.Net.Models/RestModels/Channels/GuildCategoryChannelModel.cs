using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildCategory)]
public sealed class GuildCategoryChannelModel : GuildChannelBase, IGuildCategoryChannelModel;
