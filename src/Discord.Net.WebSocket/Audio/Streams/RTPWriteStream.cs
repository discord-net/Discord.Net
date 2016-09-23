using System;
using System.IO;

namespace Discord.Audio
{
    public class RTPWriteStream : Stream
    {
        private readonly AudioClient _audioClient;
        private readonly byte[] _nonce, _secretKey;
        private int _samplesPerFrame;
        private uint _ssrc, _timestamp = 0;

        protected readonly byte[] _buffer;

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        internal RTPWriteStream(AudioClient audioClient, byte[] secretKey, int samplesPerFrame, uint ssrc, int bufferSize = 4000)
        {
            _audioClient = audioClient;
            _secretKey = secretKey;
            _samplesPerFrame = samplesPerFrame;
            _ssrc = ssrc;
            _buffer = new byte[bufferSize];
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
            _audioClient.Send(_buffer, count + 12);
        }

        public override void Flush() { }

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
