namespace Discord.Models;

public interface IGuildCreatePayloadData : IEntityModel<ulong>
{
    bool Unavailable { get; }
}
