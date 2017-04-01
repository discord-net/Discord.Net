using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public abstract class AudioInStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public abstract Task<RTPFrame> ReadFrameAsync(CancellationToken cancelToken);

        public RTPFrame ReadFrame()
        {
            return ReadFrameAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
        }
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
