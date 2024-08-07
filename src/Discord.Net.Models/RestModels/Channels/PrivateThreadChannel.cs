using Discord.Converters;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.PrivateThread)]
public sealed class PrivateThreadChannel : ThreadChannelBase;
