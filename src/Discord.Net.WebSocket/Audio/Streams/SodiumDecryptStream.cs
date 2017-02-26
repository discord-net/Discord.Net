using System;
using System.Collections.Concurrent;

namespace Discord.Audio.Streams
{
    ///<summary> Decrypts an RTP frame using libsodium </summary>
    public class SodiumDecryptStream : AudioInStream
    {
        private readonly BlockingCollection<byte[]> _queuedData; //TODO: Replace with max-length ring buffer
        private readonly AudioClient _audioClient;
        private readonly byte[] _buffer, _nonce, _secretKey;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        internal SodiumDecryptStream(AudioClient audioClient, byte[] secretKey, int bufferSize = 4000)
        {
            _audioClient = audioClient;
            _secretKey = secretKey;
            _buffer = new byte[bufferSize];
            _queuedData = new BlockingCollection<byte[]>(100);
            _nonce = new byte[24];
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var queuedData = _queuedData.Take();
            Buffer.BlockCopy(queuedData, 0, buffer, offset, Math.Min(queuedData.Length, count));
            return queuedData.Length;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            Buffer.BlockCopy(buffer, 0, _nonce, 0, 12); //Copy RTP header to nonce
            count = SecretBox.Decrypt(buffer, offset, count, _buffer, 0, _nonce, _secretKey);

            var newBuffer = new byte[count];
            Buffer.BlockCopy(_buffer, 0, newBuffer, 0, count);
            _queuedData.Add(newBuffer);
        }
    }
}
