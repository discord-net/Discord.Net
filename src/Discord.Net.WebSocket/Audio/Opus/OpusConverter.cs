using System;

namespace Discord.Audio
{
    internal abstract class OpusConverter : IDisposable
    {
        protected IntPtr _ptr;

        /// <summary> Gets the bit rate of this converter. </summary>
        public const int BitsPerSample = 16;
        /// <summary> Gets the bytes per sample. </summary>
        public const int SampleSize = (BitsPerSample / 8) * MaxChannels;
        /// <summary> Gets the maximum amount of channels this encoder supports. </summary>
        public const int MaxChannels = 2;

        /// <summary> Gets the input sampling rate of this converter. </summary>
        public int SamplingRate { get; }
        /// <summary> Gets the number of samples per second for this stream. </summary>
        public int Channels { get; }

        protected OpusConverter(int samplingRate, int channels)
        {
            if (samplingRate != 8000 && samplingRate != 12000 &&
                samplingRate != 16000 && samplingRate != 24000 &&
                samplingRate != 48000)
                throw new ArgumentOutOfRangeException(nameof(samplingRate));
            if (channels != 1 && channels != 2)
                throw new ArgumentOutOfRangeException(nameof(channels));

            SamplingRate = samplingRate;
            Channels = channels;
        }
        
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
                disposedValue = true;
        }
        ~OpusConverter()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
