using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public abstract class AudioStream : Stream
    {
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        /// <exception cref="InvalidOperationException">This stream does not accept headers.</exception>
        public virtual void WriteHeader(ushort seq, uint timestamp, bool missed) => 
            throw new InvalidOperationException("This stream does not accept headers.");
        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
        }
        public override void Flush()
        {
            FlushAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
        public void Clear()
        {
            ClearAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        public virtual Task ClearAsync(CancellationToken cancellationToken) { return Task.Delay(0); }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Reading stream length is not supported.</exception>
        public override long Length => 
            throw new NotSupportedException();

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Getting or setting this stream position is not supported.</exception>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Reading this stream is not supported.</exception>
        public override int Read(byte[] buffer, int offset, int count) =>
            throw new NotSupportedException();
        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Setting the length to this stream is not supported.</exception>
        public override void SetLength(long value) =>
            throw new NotSupportedException();
        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Seeking this stream is not supported..</exception>
        public override long Seek(long offset, SeekOrigin origin) =>
            throw new NotSupportedException();
    }
}
