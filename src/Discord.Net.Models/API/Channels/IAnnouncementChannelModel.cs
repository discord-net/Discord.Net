using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ChannelType.GuildAnnouncement)]
public interface IAnnouncementChannelModel : ITextChannelModel;