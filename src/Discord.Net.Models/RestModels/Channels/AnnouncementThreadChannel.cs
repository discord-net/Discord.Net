using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.AnnouncementThread)]
public sealed class AnnouncementThreadChannel : ThreadChannelBase;
