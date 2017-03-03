using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public abstract class AudioOutStream : Stream
    {
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

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
        //public virtual Task WriteSilenceAsync(CancellationToken cancellationToken) { return Task.Delay(0); }

        public override long Length { get { throw new NotSupportedException(); } }
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
        public override void SetLength(long value) { throw new NotSupportedException(); }
        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
    }
}
