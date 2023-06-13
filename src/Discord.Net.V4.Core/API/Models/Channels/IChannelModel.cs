namespace Discord.Models
{
    public interface IChannelModel : IEntityModel<ulong>
    {
        string Name { get; }
    }
}
