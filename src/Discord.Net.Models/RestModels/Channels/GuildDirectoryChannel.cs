using Discord.Converters;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.GuildDirectory)]
public sealed class GuildDirectoryChannel : GuildChannelBase, IGuildDirectoryChannel;
