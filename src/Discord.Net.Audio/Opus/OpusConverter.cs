using System;

namespace Discord.Audio.Opus
{
    internal abstract class OpusConverter : IDisposable
    {
        protected IntPtr _ptr;

        /// <summary> Gets the bit rate of this converter. </summary>
        public const int BitsPerSample = 16;
        /// <summary> Gets the input sampling rate of this converter. </summary>
        public int SamplingRate { get; }

        protected OpusConverter(int samplingRate)
        {
            if (samplingRate != 8000 && samplingRate != 12000 &&
                samplingRate != 16000 && samplingRate != 24000 &&
                samplingRate != 48000)
                throw new ArgumentOutOfRangeException(nameof(samplingRate));

            SamplingRate = samplingRate;
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
