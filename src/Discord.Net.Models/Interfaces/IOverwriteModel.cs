namespace Discord.Models;

public interface IOverwriteModel : IEntityModel
{
    ulong TargetId { get; }
    int Type { get; }
    ulong Allow { get; }
    ulong Deny { get; }
}
