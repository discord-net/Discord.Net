using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    internal class RTPWriteStream : AudioOutStream
    {
        private readonly IAudioTarget _target;
        private readonly byte[] _nonce, _secretKey;
        private int _samplesPerFrame;
        private uint _ssrc, _timestamp = 0;

        protected readonly byte[] _buffer;

        internal RTPWriteStream(IAudioTarget target, byte[] secretKey, int samplesPerFrame, uint ssrc)
        {
            _target = target;
            _secretKey = secretKey;
            _samplesPerFrame = samplesPerFrame;
            _ssrc = ssrc;
            _buffer = new byte[4000];
            _nonce = new byte[24];
            _nonce[0] = 0x80;
            _nonce[1] = 0x78;
            _nonce[8] = (byte)(_ssrc >> 24);
            _nonce[9] = (byte)(_ssrc >> 16);
            _nonce[10] = (byte)(_ssrc >> 8);
            _nonce[11] = (byte)(_ssrc >> 0);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
        }
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            unchecked
            {
                if (_nonce[3]++ == byte.MaxValue)
                    _nonce[2]++;

                _timestamp += (uint)_samplesPerFrame;
                _nonce[4] = (byte)(_timestamp >> 24);
                _nonce[5] = (byte)(_timestamp >> 16);
                _nonce[6] = (byte)(_timestamp >> 8);
                _nonce[7] = (byte)(_timestamp >> 0);
            }

            count = SecretBox.Encrypt(buffer, offset, count, _buffer, 12, _nonce, _secretKey);
            Buffer.BlockCopy(_nonce, 0, _buffer, 0, 12); //Copy the RTP header from nonce to buffer
            await _target.SendAsync(_buffer, count + 12).ConfigureAwait(false);
        }

        public override void Flush()
        {
            FlushAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await _target.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        public override void Clear()
        {
            ClearAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
        public override async Task ClearAsync(CancellationToken cancelToken)
        {
            await _target.ClearAsync(cancelToken).ConfigureAwait(false);
        }

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
