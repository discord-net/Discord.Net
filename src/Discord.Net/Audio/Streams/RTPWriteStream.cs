using System;
using System.IO;

namespace Discord.Audio
{
    public class RTPWriteStream : Stream
    {
        private readonly AudioClient _audioClient;
        private readonly byte[] _buffer, _nonce, _secretKey;
        private int _samplesPerFrame;
        private uint _ssrc, _timestamp = 0;

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;


        internal RTPWriteStream(AudioClient audioClient, byte[] secretKey, int samplesPerFrame, uint ssrc, int bufferSize = 4000)
        {
            _audioClient = audioClient;
            _secretKey = secretKey;
            _samplesPerFrame = samplesPerFrame;
            _ssrc = ssrc;
            _nonce = new byte[24];
            _buffer = new byte[bufferSize];
            _buffer[0] = 0x80;
            _buffer[1] = 0x78;
            _buffer[8] = (byte)(_ssrc >> 24);
            _buffer[9] = (byte)(_ssrc >> 16);
            _buffer[10] = (byte)(_ssrc >> 8);
            _buffer[11] = (byte)(_ssrc >> 0);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            unchecked
            {
                if (_buffer[3]++ == byte.MaxValue)
                    _buffer[2]++;

                _timestamp += (uint)_samplesPerFrame;
                _buffer[4] = (byte)(_timestamp >> 24);
                _buffer[5] = (byte)(_timestamp >> 16);
                _buffer[6] = (byte)(_timestamp >> 8);
                _buffer[7] = (byte)(_timestamp >> 0);
            }

            Buffer.BlockCopy(_buffer, 0, _nonce, 0, 12); //Copy the 12-byte header to be used for nonce
            count = SecretBox.Encrypt(buffer, offset, count, _buffer, 12, _nonce, _secretKey);
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
