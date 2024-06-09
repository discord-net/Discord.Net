namespace Discord.Models;

public interface IOverwriteModel
{
    ulong TargetId { get; }
    int Type { get; }
    ulong Allow { get; }
    ulong Deny { get; }
}
