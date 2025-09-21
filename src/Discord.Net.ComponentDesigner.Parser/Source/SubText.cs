using Discord.CX.Parser;
using Microsoft.CodeAnalysis.Text;
using System;

namespace Discord.CX.Parser;

partial class CXSourceText
{
    public sealed class SubText : CXSourceText
    {
        public override char this[int position]
            => _underlyingText[position + _span.Start];

        public override int Length => _span.Length;

        private readonly CXSourceText _underlyingText;
        private readonly TextSpan _span;

        public SubText(CXSourceText underlyingText, TextSpan span)
        {
            _underlyingText = underlyingText;
            _span = span;
        }

        private TextSpan GetCompositeSpan(TextSpan span)
        {
            var compositeStart = Math.Min(_underlyingText.Length, _span.Start + span.Start);
            var compositeEnd = Math.Min(_underlyingText.Length, compositeStart + span.Length);

            return TextSpan.FromBounds(compositeStart, compositeEnd);
        }

        protected override TextLineCollection ComputeLines() => new SubTextLineInfo(this);

        public override CXSourceText GetSubText(TextSpan span)
            => new SubText(_underlyingText, GetCompositeSpan(span));

        private sealed class SubTextLineInfo : TextLineCollection
        {
            public override int Count { get; }

            public override TextLine this[int index]
            {
                get
                {
                    if (index < 0 || index >= Count)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    if (_endsWithinSplitCRLF && index == Count - 1)
                        return new(_text, _text._span.End, _text._span.End);

                    var underlyingTextLine = _text._underlyingText.Lines[index + _startLineNumberInUnderlyingText];

                    var startInUnderlyingText = Math.Max(underlyingTextLine.Start, _text._span.Start);
                    var endInUnderlyingText = Math.Min(underlyingTextLine.EndIncludingBreaks, _text._span.End);

                    var startInSubText = startInUnderlyingText - _text._span.Start;
                    var resultLine = new TextLine(_text, startInUnderlyingText, endInUnderlyingText);

                    var shouldContainLineBreak = index != Count - 1;
                    var resultContainsLineBreak = resultLine.EndIncludingBreaks > resultLine.End;

                    if (shouldContainLineBreak != resultContainsLineBreak)
                        throw new InvalidOperationException();

                    return resultLine;
                }
            }

            private readonly SubText _text;

            private readonly int _startLineNumberInUnderlyingText;
            private readonly bool _startsWithinSplitCRLF;
            private readonly bool _endsWithinSplitCRLF;

            public SubTextLineInfo(SubText text)
            {
                _text = text;

                var startLineInUnderlyingText = text._underlyingText.Lines.GetLineFromPosition(text._span.Start);
                var endLineInUnderlyingText = text._underlyingText.Lines.GetLineFromPosition(text._span.End);

                _startLineNumberInUnderlyingText = startLineInUnderlyingText.LineNumber;
                Count = endLineInUnderlyingText.LineNumber - _startLineNumberInUnderlyingText + 1;

                var underlyingSpanStart = text._span.Start;
                if (
                    underlyingSpanStart == startLineInUnderlyingText.End + 1 &&
                    underlyingSpanStart == startLineInUnderlyingText.EndIncludingBreaks - 1
                )
                {
                    _startsWithinSplitCRLF = true;
                }

                var underlyingSpanEnd = text._span.End;
                if (
                    underlyingSpanEnd == endLineInUnderlyingText.End + 1 &&
                    underlyingSpanEnd == endLineInUnderlyingText.EndIncludingBreaks - 1
                )
                {
                    _endsWithinSplitCRLF = true;
                    Count++;
                }
            }

            public override int IndexOf(int position)
            {
                if (position < 0 && position > _text._span.Length)
                    throw new ArgumentOutOfRangeException(nameof(position));

                var underlyingPosition = position + _text._span.Start;
                var underlyingLineNumber = _text._underlyingText.Lines.IndexOf(underlyingPosition);

                if (_startsWithinSplitCRLF && position != 0)
                    underlyingLineNumber++;

                return underlyingLineNumber - _startLineNumberInUnderlyingText;
            }
        }
    }
}
