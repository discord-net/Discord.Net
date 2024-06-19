namespace Discord.Models;

public interface IChannelModel : IEntityModel<ulong>
{
    int Type { get; }
}
