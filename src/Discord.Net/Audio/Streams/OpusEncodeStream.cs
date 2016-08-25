namespace Discord.Audio
{
    /// <summary> A stream which encodes Opus frames as raw PCM data is written. </summary>
    public class OpusEncodeStream : RTPWriteStream
    {
        /// <summary> The sample rate of the Opus stream. </summary>
        public int SampleRate = 48000; // TODO: shouldn't these be readonly?
        /// <summary> The number of channels of the Opus stream. </summary>
        public int Channels = 2;
        
        private readonly OpusEncoder _encoder;

        internal OpusEncodeStream(AudioClient audioClient, byte[] secretKey, int samplesPerFrame, uint ssrc, int? bitrate = null, 
            OpusApplication application = OpusApplication.MusicOrMixed, int bufferSize = 4000)
            : base(audioClient, secretKey, samplesPerFrame, ssrc, bufferSize)
        {
            _encoder = new OpusEncoder(SampleRate, Channels);

            _encoder.SetForwardErrorCorrection(true);
            if (bitrate != null)
                _encoder.SetBitrate(bitrate.Value);
        }

        /// <summary> Writes Opus-encoded PCM data to the stream. </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            count = _encoder.EncodeFrame(buffer, offset, count, _buffer, 0);
            base.Write(_buffer, 0, count);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _encoder.Dispose();
        }
    }
}
