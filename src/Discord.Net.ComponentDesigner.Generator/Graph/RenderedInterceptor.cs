using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.CX;

public readonly record struct RenderedInterceptor(
    InterceptableLocation Location,
    string Source,
    ImmutableArray<Diagnostic> Diagnostics
)
{
    public bool Equals(RenderedInterceptor other)
        => Location.Data == other.Location.Data &&
           Location.Version == other.Location.Version &&
           Source == other.Source &&
           Diagnostics.SequenceEqual(other.Diagnostics);

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Location.GetHashCode();
            hashCode = (hashCode * 397) ^ Source.GetHashCode();
            hashCode = (hashCode * 397) ^ Diagnostics.Aggregate(0, (a, b) => (a * 397) ^ b.GetHashCode());
            return hashCode;
        }
    }
}
