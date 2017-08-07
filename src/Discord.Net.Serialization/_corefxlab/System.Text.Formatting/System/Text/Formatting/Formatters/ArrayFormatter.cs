// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections.Sequences;

namespace System.Text.Formatting
{
    public class ArrayFormatter : ITextOutput
    {
        ResizableArray<byte> _buffer;
        SymbolTable _symbolTable;
        ArrayPool<byte> _pool;

        public ArrayFormatter(int capacity, SymbolTable symbolTable, ArrayPool<byte> pool = null)
        {
            _pool = pool ?? ArrayPool<byte>.Shared;
            _symbolTable = symbolTable;
            _buffer = new ResizableArray<byte>(_pool.Rent(capacity));
        }

        public int CommitedByteCount => _buffer.Count;

        public void Clear() {
            _buffer.Count = 0;
        }

        public ArraySegment<byte> Free => _buffer.Free;
        public ArraySegment<byte> Formatted => _buffer.Full;

        public SymbolTable SymbolTable => _symbolTable;

        public Span<byte> Buffer => Free.AsSpan();

        void IOutput.Enlarge(int desiredBufferLength)
        {
            if (desiredBufferLength < 1) desiredBufferLength = 1;
            var doubleCount = _buffer.Free.Count * 2;
            int newSize = desiredBufferLength>doubleCount?desiredBufferLength:doubleCount;
            var newArray = _pool.Rent(newSize + _buffer.Count);
            var oldArray = _buffer.Resize(newArray);
            _pool.Return(oldArray);
        }

        void IOutput.Advance(int bytes)
        {
            _buffer.Count += bytes;
            if(_buffer.Count > _buffer.Count)
            {
                throw new InvalidOperationException("More bytes commited than returned from FreeBuffer");
            }
        }
    }
}
