using System;
using System.Runtime.InteropServices;

namespace Discord.Audio
{
    internal unsafe class OpusDecoder : OpusConverter
    {
        [DllImport("opus", EntryPoint = "opus_decoder_create", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateDecoder(int Fs, int channels, out OpusError error);
        [DllImport("opus", EntryPoint = "opus_decoder_destroy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DestroyDecoder(IntPtr decoder);
        [DllImport("opus", EntryPoint = "opus_decode", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Decode(IntPtr st, byte* data, int len, byte* pcm, int max_frame_size, int decode_fec);
        [DllImport("opus", EntryPoint = "opus_decoder_ctl", CallingConvention = CallingConvention.Cdecl)]
        private static extern int DecoderCtl(IntPtr st, OpusCtl request, int value);

        public OpusDecoder(int samplingRate, int channels)
            : base(samplingRate, channels)
        {
            OpusError error;
            _ptr = CreateDecoder(samplingRate, channels, out error);
            if (error != OpusError.OK)
                throw new Exception($"Opus Error: {error}");
        }
        
        /// <summary> Produces PCM samples from Opus-encoded audio. </summary>
        /// <param name="input">PCM samples to decode.</param>
        /// <param name="inputOffset">Offset of the frame in input.</param>
        /// <param name="output">Buffer to store the decoded frame.</param>
        public unsafe int DecodeFrame(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
            int result = 0;
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
                result = Decode(_ptr, inPtr + inputOffset, inputCount, outPtr + outputOffset, (output.Length - outputOffset) / SampleSize / MaxChannels, 0);

            if (result < 0)
                throw new Exception($"Opus Error: {(OpusError)result}");
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (_ptr != IntPtr.Zero)
            {
                DestroyDecoder(_ptr);
                _ptr = IntPtr.Zero;
            }
        }
    }
}
