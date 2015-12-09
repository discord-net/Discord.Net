using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Discord.Audio.Opus
{
	/// <summary> Opus codec wrapper. </summary>
	internal class OpusEncoder : IDisposable
    {
#if NET45
        [SuppressUnmanagedCodeSecurity]
#endif
        private unsafe static class UnsafeNativeMethods
        {
            [DllImport("opus", EntryPoint = "opus_encoder_create", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CreateEncoder(int Fs, int channels, int application, out OpusError error);
            [DllImport("opus", EntryPoint = "opus_encoder_destroy", CallingConvention = CallingConvention.Cdecl)]
            public static extern void DestroyEncoder(IntPtr encoder);
            [DllImport("opus", EntryPoint = "opus_encode", CallingConvention = CallingConvention.Cdecl)]
            public static extern int Encode(IntPtr st, byte* pcm, int frame_size, byte[] data, int max_data_bytes);
            [DllImport("opus", EntryPoint = "opus_encoder_ctl", CallingConvention = CallingConvention.Cdecl)]
            public static extern int EncoderCtl(IntPtr st, OpusCtl request, int value);
        }

        private readonly IntPtr _ptr;

		/// <summary> Gets the bit rate of the encoder. </summary>
		public const int BitsPerSample = 16;
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
		/// <summary> Gets the bit rate in kbit/s. </summary>
		public int? BitRate { get; private set; }
		/// <summary> Gets the coding mode of the encoder. </summary>
		public OpusApplication Application { get; private set; }

		/// <summary> Creates a new Opus encoder. </summary>
		/// <param name="samplingRate">Sampling rate of the input signal (Hz). Supported Values:  8000, 12000, 16000, 24000, or 48000.</param>
		/// <param name="channels">Number of channels (1 or 2) in input signal.</param>
		/// <param name="frameLength">Length, in milliseconds, that each frame takes. Supported Values: 2.5, 5, 10, 20, 40, 60</param>
		/// <param name="bitrate">Bitrate (kbit/s) used for this encoder. Supported Values: 1-512. Null will use the recommended bitrate. </param>
		/// <param name="application">Coding mode.</param>
		/// <returns>A new <c>OpusEncoder</c></returns>
		public OpusEncoder(int samplingRate, int channels, int frameLength, int? bitrate, OpusApplication application)
		{
			if (samplingRate != 8000 && samplingRate != 12000 &&
				samplingRate != 16000 && samplingRate != 24000 &&
				samplingRate != 48000)
				throw new ArgumentOutOfRangeException(nameof(samplingRate));
			if (channels != 1 && channels != 2)
				throw new ArgumentOutOfRangeException(nameof(channels));
			if (bitrate != null && (bitrate < 1 || bitrate > 512))
				throw new ArgumentOutOfRangeException(nameof(bitrate));

			InputSamplingRate = samplingRate;
			InputChannels = channels;
			Application = application;
			FrameLength = frameLength;
			SampleSize = (BitsPerSample / 8) * channels;
			SamplesPerFrame = samplingRate / 1000 * FrameLength;
			FrameSize = SamplesPerFrame * SampleSize;
			BitRate = bitrate;

			OpusError error;
			_ptr = UnsafeNativeMethods.CreateEncoder(samplingRate, channels, (int)application, out error);
			if (error != OpusError.OK)
				throw new InvalidOperationException($"Error occured while creating encoder: {error}");

			SetForwardErrorCorrection(true);
			if (bitrate != null)
				SetBitrate(bitrate.Value);
		}

		/// <summary> Produces Opus encoded audio from PCM samples. </summary>
		/// <param name="input">PCM samples to encode.</param>
		/// <param name="inputOffset">Offset of the frame in pcmSamples.</param>
		/// <param name="output">Buffer to store the encoded frame.</param>
		/// <returns>Length of the frame contained in outputBuffer.</returns>
		public unsafe int EncodeFrame(byte[] input, int inputOffset, byte[] output)
		{
			if (disposed)
				throw new ObjectDisposedException(nameof(OpusEncoder));
			
			int result = 0;
			fixed (byte* inPtr = input)
				result = UnsafeNativeMethods.Encode(_ptr, inPtr + inputOffset, SamplesPerFrame, output, output.Length);

			if (result < 0)
				throw new Exception("Encoding failed: " + ((OpusError)result).ToString());
			return result;
		}

		/// <summary> Gets or sets whether Forward Error Correction is enabled. </summary>
		public void SetForwardErrorCorrection(bool value)
		{
			if (disposed)
				throw new ObjectDisposedException(nameof(OpusEncoder));

			var result = UnsafeNativeMethods.EncoderCtl(_ptr, OpusCtl.SetInbandFECRequest, value ? 1 : 0);
			if (result < 0)
				throw new Exception("Encoder error: " + ((OpusError)result).ToString());
		}

		/// <summary> Gets or sets whether Forward Error Correction is enabled. </summary>
		public void SetBitrate(int value)
		{
			if (disposed)
				throw new ObjectDisposedException(nameof(OpusEncoder));

			var result = UnsafeNativeMethods.EncoderCtl(_ptr, OpusCtl.SetBitrateRequest, value * 1000);
			if (result < 0)
				throw new Exception("Encoder error: " + ((OpusError)result).ToString());
		}

		#region IDisposable
		private bool disposed;
		public void Dispose()
		{
			if (disposed)
				return;

			GC.SuppressFinalize(this);

			if (_ptr != IntPtr.Zero)
				UnsafeNativeMethods.DestroyEncoder(_ptr);

			disposed = true;
		}
		~OpusEncoder()
		{
			Dispose();
		}
		#endregion
	}
}