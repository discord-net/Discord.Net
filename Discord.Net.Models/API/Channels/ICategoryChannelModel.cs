using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ChannelType.GuildCategory)]
public interface ICategoryChannelModel : IGuildChannelModel;