using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ChannelType.GroupDM)]
public interface IGroupChannelModel : IChannelModel
{
    Snowflake OwnerId { get; }
    IReadOnlyList<IdOrModel<Snowflake, IUserModel>> Recipients { get; }
}