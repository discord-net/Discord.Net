using System;

namespace Discord.Audio
{
    internal abstract class OpusConverter : IDisposable
    {
        protected IntPtr _ptr;

        public const int SamplingRate = 48000;
        public const int Channels = 2;
        public const int FrameMillis = 20;

        public const int SampleBytes = sizeof(short) * Channels;

        public const int FrameSamplesPerChannel = SamplingRate / 1000 * FrameMillis;
        public const int FrameSamples = FrameSamplesPerChannel * Channels;
        public const int FrameBytes = FrameSamplesPerChannel * SampleBytes;
        
        protected bool _isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
                _isDisposed = true;
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
        
        protected static void CheckError(int result)
        {
            if (result < 0)
                throw new Exception($"Opus Error: {(OpusError)result}");
        }
        protected static void CheckError(OpusError error)
        {
            if ((int)error < 0)
                throw new Exception($"Opus Error: {error}");
        }
    }
}
