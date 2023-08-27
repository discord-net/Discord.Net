using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Discord.Gateway
{
    internal sealed class FrameStream : Stream
    {
        public override bool CanRead
            => true;

        public override bool CanSeek
            => true;

        public override bool CanWrite
            => false;

        public override long Length
            => Size;

        public override long Position
        {
            get => _index == 0 ? _position : _bufferSet[_index - 1] + _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public readonly struct BufferSource : IDisposable
        {
            public readonly RentedArray<byte> Buffer;
            public readonly int Size;

            public BufferSource(RentedArray<byte> buffer, int size)
            {
                Buffer = buffer;
                Size = size;
            }

            public void Dispose()
                => Buffer.Dispose();
        }

        public readonly int Size;

        private readonly int[] _bufferSet;

        private readonly List<BufferSource> _source;
        private int _position;
        private int _index;

        public string Hex
            => $"0x{string.Join(string.Empty, _source.Select(x => Convert.ToHexString(x.Buffer.Array[..x.Size]).Replace("-", string.Empty)))}";

        public FrameStream(List<BufferSource> source, int size)
        {
            Size = size;
            _source = source;

            _bufferSet = new int[source.Count];

            var t = 0;

            for (var i = 0; i != source.Count; i++)
            {
                _bufferSet[i] = t += source[i].Size;
            }
        }

        private void GetAddressOfTotalPosition(long target, out int index, out int position)
        {
            var track = _source.Count / 2;

            while (true)
            {
                if (track == 0)
                {
                    index = 0;
                    position = checked((int)target);
                    return;
                }

                var a = _bufferSet[track];
                var b = _bufferSet[track - 1];

                if(a >= target && target > b)
                {
                    index = track;
                    position = checked((int)(a - target));
                    return;
                }

                var comp = track / 2;

                if (comp == 0)
                    comp = 1;

                track += b > target ? -comp : comp;
            }
        }

        private unsafe int CopySegment(scoped ref Span<byte> dest, out int index, out int position)
        {
            // can we read directly
            if (_position + dest.Length <= _source[_index].Size)
            {
                _source[_index].Buffer.AsSpan()[_position..dest.Length].CopyTo(dest);
                index = _index;
                position = _position + dest.Length;
                return dest.Length;
            }

            // copy a section and increment the index
            var totalOut = 0;
            var remaining = dest.Length;

            index = _index;
            position = _position;

            if (_source[index].Size - position == 0)
            {
                // increment index
                index++;
                position = 0;
            }

            while (totalOut != dest.Length)
            {
                var source = _source[index];

                var amount = source.Size - position;

                if (remaining < amount)
                    amount = remaining;

                source.Buffer.AsSpan()[position..amount]
                    .CopyTo(dest[totalOut..amount]);

                totalOut += amount;
                remaining -= amount;

                if (remaining == 0)
                {
                    position = amount;
                    return totalOut;
                }

                position = 0;
                index++;

                if (index >= _source.Count)
                {
                    // we've read all that we could
                    return totalOut;
                }
            }

            return totalOut;
        }

        public int PeekSegment(scoped Span<byte> dest)
        {
            return CopySegment(ref dest, out _, out _);
        }

        public int ReadSegment(scoped Span<byte> dest)
        {
            var count = CopySegment(ref dest, out _index, out _position);
            return count;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // free all pins
            if (disposing)
            {
                foreach (var buffer in _source)
                    buffer.Dispose();
            }
        }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
            => ReadSegment(buffer.AsSpan()[offset..count]);

        public override int Read(Span<byte> buffer)
            => ReadSegment(buffer);

        public override long Seek(long offset, SeekOrigin origin)
        {
            var target = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => Position + offset,
                SeekOrigin.End => Size - offset,
                _ => throw new ArgumentException($"No seek origin found for: {origin}")
            };

            if (target > Size)
                throw new ArgumentOutOfRangeException(nameof(offset));

            GetAddressOfTotalPosition(target, out _index, out _position);
            return Position;
        }

        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}

