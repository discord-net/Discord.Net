// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace System.Runtime
{
    internal class ReadOnlyBufferDebuggerView<T>
    {
        private ReadOnlyBuffer<T> _buffer;

        public ReadOnlyBufferDebuggerView(ReadOnlyBuffer<T> buffer)
        {
            _buffer = buffer;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get {
                return _buffer.ToArray();
            }
        }
    }
}
