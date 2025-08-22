namespace Discord.Models;

public interface IPageMembersParams : IParametersModel
{
    Optional<int> Limit { get; }
    Optional<ulong> After { get; }
}