using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ChannelType.GuildStageVoice)]
public interface IStageChannelModel : IVoiceChannelModel;