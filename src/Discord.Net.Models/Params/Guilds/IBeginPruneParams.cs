namespace Discord.Models;

public interface IBeginPruneParams : IParametersModel
{
    Optional<int> Days { get; }
    Optional<bool> ComputePruneCount { get; }
    Optional<ICollection<ulong>> Roles { get; }
    Optional<string> Reason { get; }
}