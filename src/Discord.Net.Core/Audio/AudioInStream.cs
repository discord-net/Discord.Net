using System;
using System.IO;
using System.Threading;

namespace Discord.Audio
{
    public abstract class AudioInStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
        }

        public override void Flush() { throw new NotSupportedException(); }

        public override long Length { get { throw new NotSupportedException(); } }
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override void SetLength(long value) { throw new NotSupportedException(); }
        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
    }
}
