using System;

namespace Discord.Opus
{
	/// <summary> Opus codec wrapper. </summary>
	public class OpusEncoder : IDisposable
	{
		private readonly IntPtr _encoder;

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
		public Application Application { get; private set; }

		/// <summary> Creates a new Opus encoder. </summary>
		/// <param name="samplingRate">Sampling rate of the input signal (Hz). Supported Values:  8000, 12000, 16000, 24000, or 48000.</param>
		/// <param name="channels">Number of channels (1 or 2) in input signal.</param>
		/// <param name="frameLength">Length, in milliseconds, that each frame takes. Supported Values: 2.5, 5, 10, 20, 40, 60</param>
		/// <param name="application">Coding mode.</param>
		/// <returns>A new <c>OpusEncoder</c></returns>
		public OpusEncoder(int samplingRate, int channels, int frameLength, Application application)
		{
			if (samplingRate != 8000 && samplingRate != 12000 &&
				samplingRate != 16000 && samplingRate != 24000 &&
				samplingRate != 48000)
				throw new ArgumentOutOfRangeException("inputSamplingRate");
			if (channels != 1 && channels != 2)
				throw new ArgumentOutOfRangeException("inputChannels");

			InputSamplingRate = samplingRate;
			InputChannels = channels;
			Application = application;
			FrameLength = frameLength;
			SampleSize = (BitRate / 8) * channels;
			SamplesPerFrame = samplingRate / 1000 * FrameLength;
			FrameSize = SamplesPerFrame * SampleSize;

			Error error;
			_encoder = API.opus_encoder_create(samplingRate, channels, (int)application, out error);
			if (error != Error.OK)
				throw new InvalidOperationException("Error occured while creating encoder: " + error.ToString());

			SetForwardErrorCorrection(true);
		}

		/// <summary> Produces Opus encoded audio from PCM samples. </summary>
		/// <param name="pcmSamples">PCM samples to encode.</param>
		/// <param name="encodedLength">Length of encoded audio.</param>
		/// <returns>Opus encoded audio buffer.</returns>
		public unsafe int EncodeFrame(byte[] pcmSamples, byte[] outputBuffer)
		{
			if (disposed)
				throw new ObjectDisposedException("OpusEncoder");
			
			IntPtr encodedPtr;
			int length = 0;
			fixed (byte* bPtr = outputBuffer)
			{
				encodedPtr = new IntPtr((void*)bPtr);
				length = API.opus_encode(_encoder, pcmSamples, SamplesPerFrame, encodedPtr, outputBuffer.Length);
			}

			if (length < 0)
				throw new Exception("Encoding failed: " + ((Error)length).ToString());
			return length;
		}

		/// <summary> Gets or sets whether Forward Error Correction is enabled. </summary>
		public void SetForwardErrorCorrection(bool value)
		{
			if (_encoder == IntPtr.Zero)
				throw new ObjectDisposedException("OpusEncoder");

			var ret = API.opus_encoder_ctl(_encoder, Ctl.SetInbandFECRequest, value ? 1 : 0);
			if (ret < 0)
				throw new Exception("Encoder error - " + ((Error)ret).ToString());
		}

		private bool disposed;
		public void Dispose()
		{
			if (disposed)
				return;

			GC.SuppressFinalize(this);

			if (_encoder != IntPtr.Zero)
				API.opus_encoder_destroy(_encoder);

			disposed = true;
		}
		~OpusEncoder()
		{
			Dispose();
		}
	}
}