// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace System.Collections.Sequences
{
    public struct SequenceEnumerator<T>
    {
        Position _position;
        ISequence<T> _sequence;
        T _current;
        bool first; // this is needed so that MoveNext does not advance the first time it's called

        public SequenceEnumerator(ISequence<T> sequence) {
            _sequence = sequence;
            _position = Position.First;
            _current = default;
            first = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() {
            var result = _sequence.TryGet(ref _position, out _current, advance: !first);
            first = false;
            return result;
        }

        public T Current => _current;
    }
}
