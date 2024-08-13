using System.Numerics;

namespace Discord.Models;

[ModelEquality]
public partial interface IOverwriteModel : IEntityModel
{
    ulong TargetId { get; }
    int Type { get; }
    string Allow { get; }
    string Deny { get; }
}
