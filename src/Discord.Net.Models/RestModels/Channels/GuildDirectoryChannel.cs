using Discord.Converters;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildDirectory)]
public sealed class GuildDirectoryChannelModel : GuildChannelModelBase, IGuildDirectoryChannel;
