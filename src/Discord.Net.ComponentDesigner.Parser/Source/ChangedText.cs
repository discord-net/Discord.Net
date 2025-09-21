using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.CX.Parser;

partial class CXSourceText
{
    public sealed class ChangedText : CXSourceText
    {
        private sealed record ChangeInfo(
            ImmutableArray<TextChangeRange> Changes,
            WeakReference<CXSourceText> WeakOldText,
            ChangeInfo? Previous = null
        )
        {
            public ChangeInfo? Previous { get; private set; } = Previous;

            public void Clean()
            {
                var lastInfo = this;
                for (var info = this; info is not null; info = info.Previous)
                {
                    if (info.WeakOldText.TryGetTarget(out _))
                        lastInfo = info;
                }

                ChangeInfo? prev;
                while (lastInfo is not null)
                {
                    prev = lastInfo.Previous;
                    lastInfo.Previous = null;
                    lastInfo = prev;
                }
            }
        }

        private readonly CXSourceText _newText;
        private readonly ChangeInfo _info;

        public override char this[int position] => _newText[position];

        public override int Length => _newText.Length;

        public ChangedText(
            CXSourceText oldText,
            CXSourceText newText,
            ImmutableArray<TextChangeRange> changes)
        {
            _newText = newText;
            _info = new(changes, new(oldText), (oldText as ChangedText)?._info);
        }

        protected override TextLineCollection ComputeLines() => _newText.Lines;

        public override CXSourceText WithChanges(params IReadOnlyCollection<TextChange> changes)
        {
            var changed = _newText.WithChanges(changes);

            if (changed is ChangedText changedText)
                return new ChangedText(this, changedText._newText, changedText._info.Changes);

            return changed;
        }

        public override IReadOnlyList<TextChangeRange> GetChangeRanges(CXSourceText oldText)
        {
            if (_info.WeakOldText.TryGetTarget(out var actualOldText) && actualOldText == oldText)
                return _info.Changes;

            if (IsChangedFrom(oldText))
            {
                var changes = GetChangesBetween(oldText, this);

                if (changes.Count > 1) return Merge(changes);
            }

            if (actualOldText is not null && actualOldText.GetChangeRanges(oldText).Count == 0)
                return _info.Changes;

            return [new TextChangeRange(new(0, oldText.Length), _newText.Length)];
        }

        private bool IsChangedFrom(CXSourceText oldText)
        {
            for (var info = _info; info is not null; info = info.Previous)
            {
                if (info.WeakOldText.TryGetTarget(out var text) && text == oldText)
                    return true;
            }

            return false;
        }

        private static IReadOnlyList<ImmutableArray<TextChangeRange>> GetChangesBetween(
            CXSourceText oldText,
            ChangedText newText
        )
        {
            var results = new List<ImmutableArray<TextChangeRange>>();

            var info = newText._info;
            results.Add(info.Changes);

            while (info is not null)
            {
                info.WeakOldText.TryGetTarget(out var actualOldText);

                if (actualOldText == oldText) return results;

                if ((info = info.Previous) is not null)
                    results.Insert(0, info.Changes);
            }

            results.Clear();
            return results;
        }

        private static ImmutableArray<TextChangeRange> Merge(IReadOnlyList<ImmutableArray<TextChangeRange>> changes)
        {
            var merged = changes[0];
            for (var i = 1; i < changes.Count; i++)
            {
                merged = Merge(merged, changes[i]);
            }

            return merged;
        }

        private static ImmutableArray<TextChangeRange> Merge(
            ImmutableArray<TextChangeRange> oldChanges,
            ImmutableArray<TextChangeRange> newChanges
        )
        {
            var results = new List<TextChangeRange>();

            var oldChange = oldChanges[0];
            var newChange = new UnadjustedNewChange(newChanges[0]);

            var oldIndex = 0;
            var newIndex = 0;

            var oldDelta = 0;

            while (true)
            {
                if (oldChange is {Span.Length: 0, NewLength: 0})
                {
                    // old change doesn't insert or delete anything, so it can be discarded.
                    if (TryGetNextOldChange()) continue;

                    break;
                }

                if (newChange is {SpanLength: 0, NewLength: 0})
                {
                    // new change doesn't insert or delete anything, so it can be discarded.
                    if (TryGetNextNewChange()) continue;
                    break;
                }

                if (newChange.SpanEnd <= oldChange.Span.Start + oldDelta)
                {
                    // new change is before old change, so just take the new change
                    AdjustAndAddNewChange(results, oldDelta, newChange);

                    if (TryGetNextNewChange()) continue;

                    break;
                }

                if (newChange.SpanStart >= oldChange.Span.Start + oldChange.NewLength + oldDelta)
                {
                    // new change is after old change, so just take the old change
                    AddAndAdjustOldDelta(results, ref oldDelta, oldChange);

                    if (TryGetNextOldChange()) continue;
                    break;
                }

                if (newChange.SpanStart < oldChange.Span.Start + oldDelta)
                {
                    // new change overlaps
                    var newChangeLeadingDeletion = oldChange.Span.Start + oldDelta - newChange.SpanStart;
                    AdjustAndAddNewChange(
                        results,
                        oldDelta,
                        new(
                            newChange.SpanStart,
                            newChangeLeadingDeletion,
                            NewLength: 0
                        )
                    );
                    newChange = newChange with
                    {
                        SpanStart = oldChange.Span.Start + oldDelta,
                        SpanLength = newChange.SpanLength - newChangeLeadingDeletion,
                    };
                    continue;
                }

                if (newChange.SpanStart > oldChange.Span.Start + oldDelta)
                {
                    // new change starts after old change, but it overlaps

                    var oldChangeLeadingInsertion = newChange.SpanStart - (oldChange.Span.Start + oldDelta);
                    var oldChangeLeadingDeletion = Math.Min(oldChange.Span.Length, oldChangeLeadingInsertion);
                    AddAndAdjustOldDelta(
                        results,
                        ref oldDelta,
                        new TextChangeRange(
                            TextSpan.FromBounds(oldChange.Span.Start, oldChangeLeadingDeletion),
                            oldChangeLeadingInsertion
                        )
                    );

                    oldChange = new TextChangeRange(
                        new TextSpan(newChange.SpanStart - oldDelta, oldChange.Span.Length - oldChangeLeadingDeletion),
                        oldChange.NewLength - oldChangeLeadingInsertion
                    );
                    continue;
                }

                // old and new change start at the same position
                if (newChange.SpanLength <= oldChange.NewLength)
                {
                    // new change deletes less
                    oldChange = new(oldChange.Span, oldChange.NewLength - newChange.SpanLength);

                    oldDelta += newChange.SpanLength;
                    newChange = newChange with {SpanLength = 0};
                    AdjustAndAddNewChange(results, oldDelta, newChange);

                    if (TryGetNextNewChange()) continue;
                    break;
                }

                // new change deletes more
                oldDelta -= oldChange.Span.Length + oldChange.NewLength;

                var newDeletion = newChange.SpanLength + oldChange.Span.Length - oldChange.NewLength;
                newChange = newChange with {SpanStart = oldChange.Span.Start + oldDelta, SpanLength = newDeletion,};

                if (TryGetNextOldChange()) continue;
                break;
            }

            // there may be remaining old changes, but they're mutually exclusive
            switch (oldIndex == oldChanges.Length, newIndex == newChanges.Length)
            {
                case (true, true) or (false, false):
                    throw new InvalidOperationException();
            }

            while (oldIndex < oldChanges.Length)
            {
                AddAndAdjustOldDelta(results, ref oldDelta, oldChange);
                TryGetNextOldChange();
            }

            while (newIndex < newChanges.Length)
            {
                AdjustAndAddNewChange(results, oldDelta, newChange);
                TryGetNextNewChange();
            }

            return [..results];

            static void AddAndAdjustOldDelta(
                List<TextChangeRange> results,
                ref int oldDelta,
                TextChangeRange oldChange
            )
            {
                oldDelta -= (oldChange.Span.Length + oldChange.NewLength);
                Add(results, oldChange);
            }

            static void AdjustAndAddNewChange(
                List<TextChangeRange> results,
                int oldDelta,
                UnadjustedNewChange newChange
            )
            {
                Add(
                    results,
                    new(
                        new(newChange.SpanStart - oldDelta, newChange.SpanLength),
                        newChange.NewLength
                    )
                );
            }

            static void Add(
                List<TextChangeRange> results,
                TextChangeRange change
            )
            {
                if (results.Count == 0)
                {
                    results.Add(change);
                    return;
                }

                var last = results[^1];
                if (last.Span.End == change.Span.Start)
                {
                    // merge
                    results[^1] = new(
                        new TextSpan(last.Span.Start, last.Span.Length + change.Span.Length),
                        last.NewLength + change.NewLength
                    );
                    return;
                }

                if (last.Span.End > change.Span.Start)
                {
                    throw new ArgumentOutOfRangeException(nameof(change));
                }

                results.Add(change);
            }


            bool TryGetNextNewChange()
            {
                newIndex++;
                if (newIndex < newChanges.Length)
                {
                    newChange = new UnadjustedNewChange(newChanges[newIndex]);
                    return true;
                }

                newChange = default;
                return false;
            }

            bool TryGetNextOldChange()
            {
                oldIndex++;
                if (oldIndex < oldChanges.Length)
                {
                    oldChange = oldChanges[oldIndex];
                    return true;
                }

                oldChange = default;
                return false;
            }
        }

        private readonly record struct UnadjustedNewChange(
            int SpanStart,
            int SpanLength,
            int NewLength
        )
        {
            public int SpanEnd => SpanStart + SpanLength;

            public UnadjustedNewChange(TextChangeRange range) : this(range.Span.Start, range.Span.Length,
                range.NewLength)
            {
            }
        }
    }
}
