using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.GuildCategory)]
public sealed class GuildCategoryChannel : GuildChannelBase, IGuildCategoryChannelModel;
