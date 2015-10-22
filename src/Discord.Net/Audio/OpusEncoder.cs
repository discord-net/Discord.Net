using System;

namespace Discord.Audio
{
	/// <summary> Opus codec wrapper. </summary>
	internal class OpusEncoder : IDisposable
	{
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
		/// <summary> Gets the coding mode of the encoder. </summary>
		public Opus.Application Application { get; private set; }

		/// <summary> Creates a new Opus encoder. </summary>
		/// <param name="samplingRate">Sampling rate of the input signal (Hz). Supported Values:  8000, 12000, 16000, 24000, or 48000.</param>
		/// <param name="channels">Number of channels (1 or 2) in input signal.</param>
		/// <param name="frameLength">Length, in milliseconds, that each frame takes. Supported Values: 2.5, 5, 10, 20, 40, 60</param>
		/// <param name="application">Coding mode.</param>
		/// <returns>A new <c>OpusEncoder</c></returns>
		public OpusEncoder(int samplingRate, int channels, int frameLength, Opus.Application application)
		{
			if (samplingRate != 8000 && samplingRate != 12000 &&
				samplingRate != 16000 && samplingRate != 24000 &&
				samplingRate != 48000)
				throw new ArgumentOutOfRangeException(nameof(samplingRate));
			if (channels != 1 && channels != 2)
				throw new ArgumentOutOfRangeException(nameof(channels));

			InputSamplingRate = samplingRate;
			InputChannels = channels;
			Application = application;
			FrameLength = frameLength;
			SampleSize = (BitRate / 8) * channels;
			SamplesPerFrame = samplingRate / 1000 * FrameLength;
			FrameSize = SamplesPerFrame * SampleSize;

			Opus.Error error;
			_ptr = Opus.CreateEncoder(samplingRate, channels, (int)application, out error);
			if (error != Opus.Error.OK)
				throw new InvalidOperationException($"Error occured while creating encoder: {error}");

			SetForwardErrorCorrection(true);
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
				result = Opus.Encode(_ptr, inPtr + inputOffset, SamplesPerFrame, output, output.Length);

			if (result < 0)
				throw new Exception("Encoding failed: " + ((Opus.Error)result).ToString());
			return result;
		}

		/// <summary> Gets or sets whether Forward Error Correction is enabled. </summary>
		public void SetForwardErrorCorrection(bool value)
		{
			if (disposed)
				throw new ObjectDisposedException(nameof(OpusEncoder));

			var result = Opus.EncoderCtl(_ptr, Opus.Ctl.SetInbandFECRequest, value ? 1 : 0);
			if (result < 0)
				throw new Exception("Encoder error: " + ((Opus.Error)result).ToString());
		}

		#region IDisposable
		private bool disposed;
		public void Dispose()
		{
			if (disposed)
				return;

			GC.SuppressFinalize(this);

			if (_ptr != IntPtr.Zero)
				Opus.DestroyEncoder(_ptr);

			disposed = true;
		}
		~OpusEncoder()
		{
			Dispose();
		}
		#endregion
	}
}