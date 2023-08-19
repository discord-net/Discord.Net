using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Discord.Gateway
{
    internal unsafe ref struct FrameSource
    {
        public readonly struct BufferSource
        {
            public readonly IMemoryOwner<byte> Buffer;
            public readonly int Size;

            public BufferSource(IMemoryOwner<byte> buffer, int size)
            {
                Buffer = buffer;
                Size = size;
            }
        }

        public readonly int Size;
        public readonly int Position
            => _totalPosition;

        private readonly List<BufferSource> _source;
        private int _position;
        private int _index;
        private readonly List<MemoryHandle> _handles;

        private bool _disposed;
        private int _totalPosition;

        public FrameSource(List<BufferSource> source, int size)
        {
            Size = size;
            _source = source;
            _handles = new(source.Count);
            for(var i = 0; i != source.Count; i++)
            {
                _handles.Add(source[i].Buffer.Memory.Pin());
            }
        }

        public int ReadSegment(scoped Span<byte> dest)
        {
            // can we read directly
            if (_position + dest.Length <= _source[_index].Size)
            {
                Unsafe.CopyBlockUnaligned(ref dest[0], ref Unsafe.AsRef<byte>((byte*)_handles[_index].Pointer + _position), (uint)dest.Length);
                _position += dest.Length;
                _totalPosition += dest.Length;
                return dest.Length;
            }

            // copy a section and increment the index
            var totalOut = 0;
            var remaining = dest.Length;
            while(totalOut != dest.Length)
            {
                var part = _handles[_index];
                var source = _source[_index];

                var amount = source.Size - _position;

                if (remaining < amount)
                    amount = remaining;

                Unsafe.CopyBlockUnaligned(
                    ref dest[0],
                    ref Unsafe.AsRef<byte>((byte*)part.Pointer + _position),
                    checked((uint)amount)
                );

                totalOut += amount;
                _totalPosition += amount;

                if (remaining == 0)
                {
                    _position = amount;
                    return totalOut;
                }

                _position = 0;
                _index++;

                if(_index >= _source.Count)
                {
                    // we've read all that we could
                    return totalOut;
                }
            }

            return totalOut;
        }

        public void Dispose()
        {
            // free all pins
            if(!_disposed)
            {
                foreach (var handle in _handles)
                    handle.Dispose();
                _disposed = true;
            }
        }
    }
}

