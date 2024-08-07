namespace Discord.Models;

[ModelEquality]
public partial interface IChannelModel : IEntityModel<ulong>
{
    uint Type { get; }
}
