using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ChannelType.GuildDirectory)]
public interface IDirectoryChannelModel : IGuildChannelModel, INestedChannelModel;