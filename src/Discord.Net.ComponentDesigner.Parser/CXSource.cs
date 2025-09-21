using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace Discord.CX.Parser;

public sealed class CXSource
{
    public string Value { get; }
    public int WrappingQuoteCount { get; }

    public char this[int index] => Value[index - SourceSpan.Start];

    public readonly TextSpan[] Interpolations;
    public int Length => Value.Length;

    public readonly TextSpan SourceSpan;

    public CXSource(
        TextSpan sourceSpan,
        string content,
        TextSpan[] interpolations,
        int wrappingQuoteCount
    )
    {
        SourceSpan = sourceSpan;
        Value = content;
        WrappingQuoteCount = wrappingQuoteCount;
        Interpolations = interpolations;
    }

    public bool IsAtInterpolation(int index)
    {
        for (var i = 0; i < Interpolations.Length; i++)
            if (Interpolations[i].Contains(index))
                return true;

        return false;
    }

    public string GetValue(TextSpan span)
    {
        var start = span.Start - SourceSpan.Start;

        return Value.Substring(start, span.Length);
    }
}
