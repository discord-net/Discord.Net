using Discord.Converters;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.PublicThread)]
public sealed class PublicThreadChannel : ThreadChannelBase;
