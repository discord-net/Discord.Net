using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Discord.Audio.Opus
{
	/// <summary> Opus codec wrapper. </summary>
	internal class OpusDecoder : IDisposable
    {
#if NET45
        [SuppressUnmanagedCodeSecurity]
#endif
        private unsafe static class UnsafeNativeMethods
        {
            [DllImport("opus", EntryPoint = "opus_decoder_create", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CreateDecoder(int Fs, int channels, out OpusError error);
            [DllImport("opus", EntryPoint = "opus_decoder_destroy", CallingConvention = CallingConvention.Cdecl)]
            public static extern void DestroyDecoder(IntPtr decoder);
            [DllImport("opus", EntryPoint = "opus_decode", CallingConvention = CallingConvention.Cdecl)]
            public static extern int Decode(IntPtr st, byte* data, int len, byte[] pcm, int frame_size, int decode_fec);
        }

        private readonly IntPtr _ptr;

		/// <summary> Gets the bit rate of the encoder. </summary>
		public const int BitRate = 16;
		/// <summary> Gets the input sampling rate of the encoder. </summary>
		public int InputSamplingRate { get; private set; }
		/// <summary> Gets the number of channels of the encoder. </summary>
		public int InputChannels { get; private set; }
		/// <summary> Gets the milliseconds per frame. </summary>
		public int FrameLength { get; private set; }
		/// <summary> Gets the number of samples per frame. </summary>
		public int SamplesPerFrame { get; private set; }
		/// <summary> Gets the bytes per sample. </summary>
		public int SampleSize { get; private set; }
		/// <summary> Gets the bytes per frame. </summary>
		public int FrameSize { get; private set; }

		/// <summary> Creates a new Opus decoder. </summary>
		/// <param name="samplingRate">Sampling rate of the input signal (Hz). Supported Values:  8000, 12000, 16000, 24000, or 48000.</param>
		/// <param name="channels">Number of channels (1 or 2) in input signal.</param>
		/// <param name="frameLength">Length, in milliseconds, that each frame takes. Supported Values: 2.5, 5, 10, 20, 40, 60</param>
		/// <param name="application">Coding mode.</param>
		/// <returns>A new <c>OpusEncoder</c></returns>
		public OpusDecoder(int samplingRate, int channels, int frameLength)
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
			SampleSize = (BitRate / 8) * channels;
			SamplesPerFrame = samplingRate / 1000 * FrameLength;
			FrameSize = SamplesPerFrame * SampleSize;

			OpusError error;
			_ptr = UnsafeNativeMethods.CreateDecoder(samplingRate, channels,  out error);
			if (error != OpusError.OK)
				throw new InvalidOperationException($"Error occured while creating decoder: {error}");
		}

        /// <summary> Produces PCM samples from Opus-encoded audio. </summary>
        /// <param name="input">PCM samples to decode.</param>
        /// <param name="inputOffset">Offset of the frame in input.</param>
        /// <param name="output">Buffer to store the decoded frame.</param>
        /// <returns>Length of the frame contained in output.</returns>
        public unsafe int DecodeFrame(byte[] input, int inputOffset, byte[] output)
		{
			if (disposed)
				throw new ObjectDisposedException(nameof(OpusDecoder));
			
			int result = 0;
			fixed (byte* inPtr = input)
				result = UnsafeNativeMethods.Decode(_ptr, inPtr + inputOffset, SamplesPerFrame, output, output.Length, 0);

			if (result < 0)
				throw new Exception("Decoding failed: " + ((OpusError)result).ToString());
			return result;
		}

#region IDisposable
		private bool disposed;
		public void Dispose()
		{
			if (disposed)
				return;

			GC.SuppressFinalize(this);

			if (_ptr != IntPtr.Zero)
				UnsafeNativeMethods.DestroyDecoder(_ptr);

			disposed = true;
		}
		~OpusDecoder()
		{
			Dispose();
		}
#endregion
	}
}