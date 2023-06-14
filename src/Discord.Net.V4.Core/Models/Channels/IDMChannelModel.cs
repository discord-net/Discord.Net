namespace Discord.Models
{
    public interface IDMChannelModel : IChannelModel
    {
        ulong RecipientId { get; }
    }
}
