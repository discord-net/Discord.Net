using Microsoft.CodeAnalysis;

namespace Discord.CX;

public readonly record struct InterpolationInfo(
    int Id,
    int Length,
    ITypeSymbol Type
)
{
    public bool Equals(InterpolationInfo? other)
        => other is { } info &&
           Id == info.Id &&
           Length == info.Length &&
           Type.ToDisplayString() == info.Type.ToDisplayString();

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Id;
            hashCode = (hashCode * 397) ^ Length;
            hashCode = (hashCode * 397) ^ Type.ToDisplayString().GetHashCode();
            return hashCode;
        }
    }
}
