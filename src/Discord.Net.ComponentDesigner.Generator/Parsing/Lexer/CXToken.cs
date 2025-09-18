using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed record CXToken(
    CXTokenKind Kind,
    TextSpan Span,
    int LeadingTriviaLength,
    int TrailingTriviaLength,
    CXTokenFlags Flags,
    string Value,
    params IReadOnlyList<CXDiagnostic> Diagnostics
) : ICXNode
{
    public CXNode? Parent { get; set; }

    public bool HasErrors
        => _hasErrors ??= (
            Kind is CXTokenKind.Invalid ||
            Diagnostics.Any(x => x.Severity is DiagnosticSeverity.Error) ||
            (Flags & CXTokenFlags.Missing) != 0
        );

    public bool IsMissing => (Flags & CXTokenFlags.Missing) != 0;

    public bool IsZeroWidth => Span.IsEmpty;

    public bool IsInvalid => Kind is CXTokenKind.Invalid;

    public int AbsoluteStart => Span.Start - LeadingTriviaLength;
    public int AbsoluteEnd => Span.End + TrailingTriviaLength;

    public int AbsoluteWidth => AbsoluteEnd - AbsoluteStart;

    public TextSpan FullSpan => new(AbsoluteStart, AbsoluteWidth);

    public int Width => FullSpan.Length;

    int ICXNode.GraphWidth => 0;
    IReadOnlyList<CXNode.ParseSlot> ICXNode.Slots => [];

    private bool? _hasErrors;

    public void ResetCachedState()
    {
        _hasErrors = null;
    }

    public bool Equals(CXToken? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return
            Kind == other.Kind &&
            Span.Equals(other.Span) &&
            LeadingTriviaLength == other.LeadingTriviaLength &&
            TrailingTriviaLength == other.TrailingTriviaLength &&
            Flags == other.Flags &&
            Diagnostics.SequenceEqual(other.Diagnostics);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Diagnostics.Aggregate(0, (a, b) => (a * 397) ^ b.GetHashCode());
            hashCode = (hashCode * 397) ^ (int)Kind;
            hashCode = (hashCode * 397) ^ Span.GetHashCode();
            hashCode = (hashCode * 397) ^ LeadingTriviaLength;
            hashCode = (hashCode * 397) ^ TrailingTriviaLength;
            hashCode = (hashCode * 397) ^ (int)Flags;
            return hashCode;
        }
    }
}
