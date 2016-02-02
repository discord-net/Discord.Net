using System;

namespace Discord.Audio.Opus
{
	internal class OpusDecoder : OpusConverter
    {
        /// <summary> Creates a new Opus decoder. </summary>
        /// <param name="samplingRate">Sampling rate of the input PCM (in Hz). Supported Values:  8000, 12000, 16000, 24000, or 48000</param>
        /// <param name="frameLength">Length, in milliseconds, of each frame. Supported Values: 2.5, 5, 10, 20, 40, or 60</param>
        public OpusDecoder(int samplingRate, int channels, int frameLength)
            : base(samplingRate, channels, frameLength)
		{
			OpusError error;
			_ptr = UnsafeNativeMethods.CreateDecoder(samplingRate, channels,  out error);
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
				result = UnsafeNativeMethods.Decode(_ptr, inPtr + inputOffset, inputCount, output, SamplesPerFrame, 0);

			if (result < 0)
				throw new Exception(((OpusError)result).ToString());
			return result;
		}

        protected override void Dispose(bool disposing)
        {
            if (_ptr != IntPtr.Zero)
            {
                UnsafeNativeMethods.DestroyDecoder(_ptr);
                _ptr = IntPtr.Zero;
            }
        }
	}
}