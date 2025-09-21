using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.CX.Parser;

partial class CXSourceText
{
    public sealed class CompositeText : CXSourceText
    {
        public override char this[int position]
        {
            get
            {
                GetIndexAndOffset(position, out var index, out var offset);
                return _segments[index][offset];
            }
        }

        public override int Length { get; }

        private readonly ImmutableArray<CXSourceText> _segments;
        private readonly CXSourceText _original;
        private readonly int[] _offsets;

        public CompositeText(ImmutableArray<CXSourceText> segments, CXSourceText original)
        {
            _segments = segments;
            _original = original;

            _offsets = new int[segments.Length];

            for (var i = 0; i < segments.Length; i++)
            {
                _offsets[i] = Length;
                Length += segments[i].Length;
            }
        }

        private void GetIndexAndOffset(int pos, out int index, out int offset)
        {
            var idx = BinSearchOffsets(pos);
            index = idx >= 0 ? idx : (~idx - 1);
            offset = pos - _offsets[index];
        }

        private int BinSearchOffsets(int pos)
        {
            var low = 0;
            var high = _offsets.Length - 1;

            while (low <= high)
            {
                var mid = low + ((high - low) >> 1);
                var midVal = _offsets[mid];

                if (midVal == pos) return mid;

                if (midVal > pos)
                {
                    high = mid - 1;
                    continue;
                }

                low = mid + 1;
            }

            return ~low;
        }


        public static CompositeText Create(
            ImmutableArray<CXSourceText> segments,
            CXSourceText original
        ) => new(segments, original);

        public static void AddSegments(List<CXSourceText> segments, CXSourceText text)
        {
            if (text is CompositeText composite)
                segments.AddRange(composite._segments);
            else segments.Add(text);
        }

        private sealed class CompositeTextLineInfo : TextLineCollection
        {
            public override int Count => _lineCount;

            public override TextLine this[int index]
            {
                get
                {
                    if (index < 0 || index >= _lineCount)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    GetSegmentIndexRangeContainingLine(
                        index,
                        out var firstSegmentIndexInclusive,
                        out var lastSegmentIndexInclusive
                    );

                    var firstSegmentFirstLineNumber = _segmentLineNumbers[firstSegmentIndexInclusive];
                    var firstSegment = _text._segments[firstSegmentIndexInclusive];
                    var firstSegmentOffset = _text._offsets[firstSegmentIndexInclusive];
                    var firstSegmentTextLine = firstSegment.Lines[index - firstSegmentFirstLineNumber];

                    var lineLength = firstSegmentTextLine.SpanIncludingBreaks.Length;

                    for (
                        var nextSegmentIndex = firstSegmentIndexInclusive + 1;
                        nextSegmentIndex < lastSegmentIndexInclusive;
                        nextSegmentIndex++
                    )
                    {
                        var nextSegment = _text._segments[nextSegmentIndex];

                        lineLength += nextSegment.Lines[0].SpanIncludingBreaks.Length;
                    }

                    if (firstSegmentIndexInclusive != lastSegmentIndexInclusive)
                    {
                        var lastSegment = _text._segments[lastSegmentIndexInclusive];
                        lineLength += lastSegment.Lines[0].SpanIncludingBreaks.Length;
                    }

                    return new TextLine(
                        _text,
                        firstSegmentOffset + firstSegmentTextLine.Start,
                        firstSegmentOffset + firstSegmentTextLine.Start + lineLength
                    );
                }
            }

            private readonly CompositeText _text;
            private readonly ImmutableArray<int> _segmentLineNumbers;
            private readonly int _lineCount;

            public CompositeTextLineInfo(CompositeText text)
            {
                var segmentLineNumbers = new int[text._segments.Length];
                var accumulatedLineCount = 0;

                for (var i = 0; i < text._segments.Length; i++)
                {
                    segmentLineNumbers[i] = accumulatedLineCount;

                    var segment = text._segments[i];
                    accumulatedLineCount += segment.Lines.Count;
                }

                _segmentLineNumbers = [..segmentLineNumbers];
                _text = text;
                _lineCount = accumulatedLineCount + 1;
            }

            public override int IndexOf(int position)
            {
                if (position < 0 || position >= _text.Length)
                    throw new ArgumentOutOfRangeException(nameof(position));

                _text.GetIndexAndOffset(position, out var index, out var offset);

                var segment = _text._segments[index];
                var lineNumberWithinSegment = segment.Lines.IndexOf(offset);

                return _segmentLineNumbers[index] + lineNumberWithinSegment;
            }

            private void GetSegmentIndexRangeContainingLine(
                int lineNumber,
                out int firstSegmentIndexInclusive,
                out int lastSegmentIndexInclusive
            )
            {
                var idx = _segmentLineNumbers.BinarySearch(lineNumber);
                var binarySearchSegmentIndex = idx >= 0 ? idx : (~idx - 1);

                for (
                    firstSegmentIndexInclusive = binarySearchSegmentIndex;
                    firstSegmentIndexInclusive > 0;
                    firstSegmentIndexInclusive--
                )
                {
                    if (_segmentLineNumbers[firstSegmentIndexInclusive] != lineNumber)
                        break;

                    var previousSegment = _text._segments[firstSegmentIndexInclusive - 1];
                    var previousSegmentLastChar = previousSegment[^1];

                    if (previousSegmentLastChar.IsNewline()) break;
                }

                for (
                    lastSegmentIndexInclusive = binarySearchSegmentIndex;
                    lastSegmentIndexInclusive < _text._segments.Length - 1;
                    lastSegmentIndexInclusive++
                )
                {
                    if (_segmentLineNumbers[lastSegmentIndexInclusive + 1] != lineNumber)
                        break;
                }
            }
        }
    }
}
