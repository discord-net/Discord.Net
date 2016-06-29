namespace Discord.Audio
{
    public class OpusEncodeStream : RTPWriteStream
    {
        private readonly byte[] _buffer;
        private readonly OpusEncoder _encoder;

        internal OpusEncodeStream(AudioClient audioClient, byte[] secretKey, int samplesPerFrame, uint ssrc, int samplingRate, int? bitrate = null, 
            int channels = OpusConverter.MaxChannels,  OpusApplication application = OpusApplication.MusicOrMixed, int bufferSize = 4000)
            : base(audioClient, secretKey, samplesPerFrame, ssrc)
        {
            _buffer = new byte[bufferSize];
            _encoder = new OpusEncoder(samplingRate, channels);

            _encoder.SetForwardErrorCorrection(true);
            if (bitrate != null)
                _encoder.SetBitrate(bitrate.Value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            count = _encoder.EncodeFrame(buffer, offset, count, _buffer, 0);
            base.Write(_buffer, 0, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _encoder.Dispose();
        }
    }
}
