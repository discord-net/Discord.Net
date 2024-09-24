using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.GuildAnnouncement)]
public sealed class GuildAnnouncementChannel : GuildTextChannel, IGuildNewsChannelModel;
