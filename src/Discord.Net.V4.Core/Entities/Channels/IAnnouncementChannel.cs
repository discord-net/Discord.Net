using Discord.Models;

namespace Discord;

public partial interface IAnnouncementChannel :
    ISnowflakeEntity<IGuildNewsChannelModel>,
    ITextChannel,
    IAnnouncementChannelActor,
    IUpdatable<IGuildNewsChannelModel>
{
    [SourceOfTruth]
    new IGuildNewsChannelModel GetModel();
}
