namespace Discord.Audio
{
    /// <summary> A stream which decodes Opus frames as they are read. </summary>
    public class OpusDecodeStream : RTPReadStream
    {
        private readonly byte[] _buffer;
        private readonly OpusDecoder _decoder;

        internal OpusDecodeStream(AudioClient audioClient, byte[] secretKey, int samplingRate, 
            int channels = OpusConverter.MaxChannels, int bufferSize = 4000)
            : base(audioClient, secretKey)
        {
            _buffer = new byte[bufferSize];
            _decoder = new OpusDecoder(samplingRate, channels);
        }

        /// <summary> Reads Opus-encoded frame from the stream, filling the buffer with PCM data </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            count = _decoder.DecodeFrame(buffer, offset, count, _buffer, 0);
            return base.Read(_buffer, 0, count);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _decoder.Dispose();
        }
    }
}
