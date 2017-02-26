using System;
using System.Collections.Concurrent;
using System.IO;

namespace Discord.Audio.Streams
{
    ///<summary> Reads the payload from an RTP frame </summary>
    public class RTPReadStream : AudioInStream
    {
        private readonly BlockingCollection<byte[]> _queuedData; //TODO: Replace with max-length ring buffer
        //private readonly BlockingCollection<RTPFrame> _queuedData; //TODO: Replace with max-length ring buffer
        private readonly AudioClient _audioClient;
        private readonly byte[] _buffer, _nonce, _secretKey;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        internal RTPReadStream(AudioClient audioClient, byte[] secretKey, int bufferSize = 4000)
        {
            _audioClient = audioClient;
            _secretKey = secretKey;
            _buffer = new byte[bufferSize];
            _queuedData = new BlockingCollection<byte[]>(100);
            _nonce = new byte[24];
        }

        /*public RTPFrame ReadFrame()
        {          
            var queuedData = _queuedData.Take();
            Buffer.BlockCopy(queuedData, 0, buffer, offset, Math.Min(queuedData.Length, count));
            return queuedData.Length;  
        }*/
        public override int Read(byte[] buffer, int offset, int count)
        {
            var queuedData = _queuedData.Take();
            Buffer.BlockCopy(queuedData, 0, buffer, offset, Math.Min(queuedData.Length, count));
            return queuedData.Length;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            var newBuffer = new byte[count];
            Buffer.BlockCopy(buffer, 0, newBuffer, 0, count);
            _queuedData.Add(newBuffer);
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
