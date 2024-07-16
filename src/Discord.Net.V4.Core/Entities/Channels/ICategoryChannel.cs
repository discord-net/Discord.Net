using Discord.Models;

namespace Discord;

/// <summary>
///     Represents a generic category channel.
/// </summary>
public partial interface ICategoryChannel :
    ISnowflakeEntity<IGuildCategoryChannelModel>,
    IGuildChannel,
    IUpdatable<IGuildCategoryChannelModel>
{
    [SourceOfTruth]
    new IGuildCategoryChannelModel GetModel();
}
