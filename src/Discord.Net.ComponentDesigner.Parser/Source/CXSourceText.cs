using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;

namespace Discord.CX.Parser;

public abstract partial class CXSourceText
{
    public abstract int Length { get; }

    public abstract char this[int position] { get; }

    public virtual string this[TextSpan span] => this[span.Start, span.Length];

    public virtual string this[int position, int length]
    {
        get
        {
            var slice = new char[length];

            for(var i = 0; i < length; i++)
                slice[i] = this[position + i];

            return new string(slice);
        }
    }

    public TextLineCollection Lines => _lines ??= ComputeLines();
    private TextLineCollection? _lines;

    public virtual CXSourceText GetSubText(TextSpan span)
    {
        if (span.Length == 0) return new StringSource(string.Empty);

        if (span.Length == Length && span.Start == 0) return this;

        return new SubText(this, span);
    }

    public virtual CXSourceText WithChanges(params IReadOnlyCollection<TextChange> changes)
    {
        if (changes.Count == 0) return this;

        var segments = new List<CXSourceText>();
        var changeRanges = new List<TextChangeRange>();

        var pos = 0;
        foreach (var change in changes)
        {
            if (change.Span.Start < pos)
            {
                if (change.Span.End <= changeRanges.Last().Span.Start)
                {
                    return WithChanges(
                        changes
                            .Where(x => !x.Span.IsEmpty || x.NewText?.Length > 0)
                            .OrderBy(x => x.Span)
                            .ToList()
                    );
                }

                throw new InvalidOperationException("Changes cannot overlap.");
            }

            var newTextLength = change.NewText?.Length ?? 0;

            if (change.Span.Length == 0 && newTextLength == 0)
                continue;

            if (change.Span.Start > pos)
            {
                var sub = GetSubText(new(pos, change.Span.Start));
                CompositeText.AddSegments(segments, sub);
            }

            if (newTextLength > 0)
            {
                var segment = new StringSource(change.NewText!);
                CompositeText.AddSegments(segments, segment);
            }

            pos = change.Span.End;
            changeRanges.Add(new(change.Span, newTextLength));
        }

        if (pos == 0 && segments.Count == 0) return this;

        if (pos < Length)
        {
            var subText = GetSubText(new(pos, Length));
            CompositeText.AddSegments(segments, subText);
        }

        var newText = CompositeText.Create([..segments], this);

        return new ChangedText(this, newText, [..changeRanges]);
    }

    public virtual IReadOnlyList<TextChangeRange> GetChangeRanges(CXSourceText oldText)
    {
        if (oldText == this) return [];

        return [new TextChangeRange(new(0, oldText.Length), Length)];
    }

    public CXSourceText Replace(TextSpan span, string? newText)
        => WithChanges(new TextChange(span, newText ?? string.Empty));

    public virtual IReadOnlyList<TextChange> GetTextChanges(CXSourceText oldText)
    {
        var newPosDelta = 0;

        var ranges = GetChangeRanges(oldText);
        var results = new List<TextChange>();

        foreach (var range in ranges)
        {
            var newPos = range.Span.Start + newPosDelta;

            var text = range.NewLength > 0
                ? this[new TextSpan(newPos, range.NewLength)].ToString()
                : string.Empty;

            results.Add(new(range.Span, text));
            newPosDelta += range.NewLength - range.Span.Length;
        }

        return results;
    }

    protected virtual TextLineCollection ComputeLines()
        => new LineInfo(this, ParseLineOffsets());

    private ImmutableArray<int> ParseLineOffsets()
    {
        if (Length == 0) return [0];

        var lineStarts = new List<int>(Length / 64) {0};

        for (var i = 0; i < Length; i++)
        {
            var ch = this[i];

            const uint bias = '\r' + 1;
            if (unchecked(ch - bias) <= 127 - bias)
                continue;

            if (ch is '\r')
            {
                if (Length == i + 1)
                    break;

                if (this[i + 1] is '\n')
                {
                    i += 2;
                    lineStarts.Add(i);
                    continue;
                }

                lineStarts.Add(i + 1);
                continue;
            }

            if (!ch.IsNewline()) continue;

            lineStarts.Add(i + 1);
        }

        return [..lineStarts];
    }

    private sealed class LineInfo : TextLineCollection
    {
        public override int Count => _lineOffsets.Length;

        public override TextLine this[int index]
        {
            get
            {
                if (index < 0 || index >= _lineOffsets.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var start = _lineOffsets[index];

                var end = index == _lineOffsets.Length - 1
                    ? _source.Length
                    : _lineOffsets[index + 1];

                return new(_source, start, end);
            }
        }

        private readonly CXSourceText _source;
        private readonly ImmutableArray<int> _lineOffsets;

        public LineInfo(CXSourceText source, ImmutableArray<int> lineOffsets)
        {
            _source = source;
            _lineOffsets = lineOffsets;
        }

        public override int IndexOf(int position)
        {
            if (position < 0 || position > _source.Length)
                throw new ArgumentOutOfRangeException(nameof(position));

            var lineNumber = _lineOffsets.BinarySearch(position);

            if (lineNumber < 0) lineNumber = ~lineNumber - 1;

            return lineNumber;
        }
    }
}
