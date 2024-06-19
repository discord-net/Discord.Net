using System.Numerics;

namespace Discord.Models;

public interface IOverwriteModel : IEntityModel
{
    ulong TargetId { get; }
    int Type { get; }
    BigInteger Allow { get; }
    BigInteger Deny { get; }
}
