using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ChannelType.DM)]
public interface IDMChannelModel : IChannelModel
{
    IdOrModel<Snowflake, IUserModel> Recipient { get; }
}