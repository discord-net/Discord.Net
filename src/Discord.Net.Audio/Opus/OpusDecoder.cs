using System;
using System.Runtime.InteropServices;

namespace Discord.Audio.Opus
{
    internal unsafe class OpusDecoder : OpusConverter
    {
        [DllImport("opus", EntryPoint = "opus_decoder_create", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateDecoder(int Fs, int channels, out OpusError error);
        [DllImport("opus", EntryPoint = "opus_decoder_destroy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DestroyDecoder(IntPtr decoder);
        [DllImport("opus", EntryPoint = "opus_decode", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Decode(IntPtr st, byte* data, int len, byte[] pcm, int frame_size, int decode_fec);

        public OpusDecoder(int samplingRate)
            : base(samplingRate)
        {
            OpusError error;
            _ptr = CreateDecoder(samplingRate, 2, out error);
            if (error != OpusError.OK)
                throw new InvalidOperationException($"Error occured while creating decoder: {error}");
        }
        
        /// <summary> Produces PCM samples from Opus-encoded audio. </summary>
        /// <param name="input">PCM samples to decode.</param>
        /// <param name="inputOffset">Offset of the frame in input.</param>
        /// <param name="output">Buffer to store the decoded frame.</param>
        public unsafe int DecodeFrame(byte[] input, int inputOffset, int inputCount, byte[] output)
        {
            int result = 0;
            fixed (byte* inPtr = input)
                result = Decode(_ptr, inPtr + inputOffset, inputCount, output, inputCount, 0);

            if (result < 0)
                throw new Exception(((OpusError)result).ToString());
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
