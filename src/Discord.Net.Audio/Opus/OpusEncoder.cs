using System;

namespace Discord.Audio.Opus
{
	internal class OpusEncoder : OpusConverter
    {
        /// <summary> Gets the bit rate in kbit/s. </summary>
        public int? BitRate { get; }
		/// <summary> Gets the coding mode of the encoder. </summary>
		public OpusApplication Application { get; }

        /// <summary> Creates a new Opus encoder. </summary>
        /// <param name="samplingRate">Sampling rate of the input signal (Hz). Supported Values:  8000, 12000, 16000, 24000, or 48000</param>
        /// <param name="channels">Number of channels in input signal. Supported Values: 1 or 2</param>
        /// <param name="frameLength">Length, in milliseconds, that each frame takes. Supported Values: 2.5, 5, 10, 20, 40, 60</param>
        /// <param name="bitrate">Bitrate (kbit/s) used for this encoder. Supported Values: 1-512. Null will use the recommended bitrate. </param>
        /// <param name="application">Coding mode.</param>
        public OpusEncoder(int samplingRate, int channels, int frameLength, int? bitrate, OpusApplication application)
            : base(samplingRate, channels, frameLength)
		{
			if (bitrate != null && (bitrate < 1 || bitrate > AudioServiceConfig.MaxBitrate))
				throw new ArgumentOutOfRangeException(nameof(bitrate));

			BitRate = bitrate;
            Application = application;

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
			int result = 0;
			fixed (byte* inPtr = input)
				result = UnsafeNativeMethods.Encode(_ptr, inPtr + inputOffset, SamplesPerFrame, output, output.Length);

			if (result < 0)
				throw new Exception(((OpusError)result).ToString());
			return result;
		}

		/// <summary> Gets or sets whether Forward Error Correction is enabled. </summary>
		public void SetForwardErrorCorrection(bool value)
		{
			var result = UnsafeNativeMethods.EncoderCtl(_ptr, Ctl.SetInbandFECRequest, value ? 1 : 0);
			if (result < 0)
				throw new Exception(((OpusError)result).ToString());
		}

		/// <summary> Gets or sets whether Forward Error Correction is enabled. </summary>
		public void SetBitrate(int value)
		{
			var result = UnsafeNativeMethods.EncoderCtl(_ptr, Ctl.SetBitrateRequest, value * 1000);
			if (result < 0)
				throw new Exception(((OpusError)result).ToString());
		}

        protected override void Dispose(bool disposing)
        {
            if (_ptr != IntPtr.Zero)
            {
                UnsafeNativeMethods.DestroyEncoder(_ptr);
                _ptr = IntPtr.Zero;
            }
        }
	}
}