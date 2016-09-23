namespace Discord.Audio
{
    public class OpusEncodeStream : RTPWriteStream
    {
        public int SampleRate = 48000;
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
