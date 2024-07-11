namespace Discord.Models;

[ModelEquality]
public partial interface IDMChannelModel : IChannelModel
{
    ulong RecipientId { get; }
}
