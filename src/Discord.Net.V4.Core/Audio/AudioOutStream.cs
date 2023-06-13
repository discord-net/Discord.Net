using System;
using System.IO;

namespace Discord.Audio
{
    public abstract class AudioOutStream : AudioStream
    {
        public override bool CanWrite => true;

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
