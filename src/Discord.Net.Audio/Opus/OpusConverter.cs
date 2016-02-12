using System;
using System.Runtime.InteropServices;
#if NET45
using System.Security;
#endif

namespace Discord.Audio.Opus
{
    internal enum OpusApplication : int
    {
        Voice = 2048,
        MusicOrMixed = 2049,
        LowLatency = 2051
    }
    internal enum OpusError : int
    {
        OK = 0,
        BadArg = -1,
        BufferToSmall = -2,
        InternalError = -3,
        InvalidPacket = -4,
        Unimplemented = -5,
        InvalidState = -6,
        AllocFail = -7
    }

    internal abstract class OpusConverter : IDisposable
    {
        protected enum Ctl : int
        {
            SetBitrateRequest = 4002,
            GetBitrateRequest = 4003,
            SetInbandFECRequest = 4012,
            GetInbandFECRequest = 4013
        }

#if NET45
        [SuppressUnmanagedCodeSecurity]
#endif
        protected unsafe static class UnsafeNativeMethods
        {
            [DllImport("opus", EntryPoint = "opus_encoder_create", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CreateEncoder(int Fs, int channels, int application, out OpusError error);
            [DllImport("opus", EntryPoint = "opus_encoder_destroy", CallingConvention = CallingConvention.Cdecl)]
            public static extern void DestroyEncoder(IntPtr encoder);
            [DllImport("opus", EntryPoint = "opus_encode", CallingConvention = CallingConvention.Cdecl)]
            public static extern int Encode(IntPtr st, byte* pcm, int frame_size, byte[] data, int max_data_bytes);
            [DllImport("opus", EntryPoint = "opus_encoder_ctl", CallingConvention = CallingConvention.Cdecl)]
            public static extern int EncoderCtl(IntPtr st, Ctl request, int value);

            [DllImport("opus", EntryPoint = "opus_decoder_create", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CreateDecoder(int Fs, int channels, out OpusError error);
            [DllImport("opus", EntryPoint = "opus_decoder_destroy", CallingConvention = CallingConvention.Cdecl)]
            public static extern void DestroyDecoder(IntPtr decoder);
            [DllImport("opus", EntryPoint = "opus_decode", CallingConvention = CallingConvention.Cdecl)]
            public static extern int Decode(IntPtr st, byte* data, int len, byte[] pcm, int frame_size, int decode_fec);
        }

        protected IntPtr _ptr;

        /// <summary> Gets the bit rate of this converter. </summary>
        public const int BitsPerSample = 16;
        /// <summary> Gets the input sampling rate of this converter. </summary>
        public int InputSamplingRate { get; }
        /// <summary> Gets the number of channels of this converter. </summary>
        public int InputChannels { get; }
        /// <summary> Gets the milliseconds per frame. </summary>
        public int FrameLength { get; }
        /// <summary> Gets the number of samples per frame. </summary>
        public int SamplesPerFrame { get; }
        /// <summary> Gets the bytes per frame. </summary>
        public int FrameSize { get; }
        /// <summary> Gets the bytes per sample. </summary>
        public int SampleSize { get; }

        protected OpusConverter(int samplingRate, int channels, int frameLength)
        {
            if (samplingRate != 8000 && samplingRate != 12000 &&
                samplingRate != 16000 && samplingRate != 24000 &&
                samplingRate != 48000)
                throw new ArgumentOutOfRangeException(nameof(samplingRate));
            if (channels != 1 && channels != 2)
                throw new ArgumentOutOfRangeException(nameof(channels));

            InputSamplingRate = samplingRate;
            InputChannels = channels;
            FrameLength = frameLength;
            SampleSize = (BitsPerSample / 8) * channels;
            SamplesPerFrame = samplingRate / 1000 * FrameLength;
            FrameSize = SamplesPerFrame * SampleSize;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
                disposedValue = true;
        }        
        ~OpusConverter() {
           Dispose(false);
        }        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
