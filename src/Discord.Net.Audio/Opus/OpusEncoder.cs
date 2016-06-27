using System;
using System.Runtime.InteropServices;

namespace Discord.Audio.Opus
{
    internal unsafe class OpusEncoder : OpusConverter
    {
        [DllImport("opus", EntryPoint = "opus_encoder_create", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateEncoder(int Fs, int channels, int application, out OpusError error);
        [DllImport("opus", EntryPoint = "opus_encoder_destroy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DestroyEncoder(IntPtr encoder);
        [DllImport("opus", EntryPoint = "opus_encode", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Encode(IntPtr st, byte* pcm, int frame_size, byte[] data, int max_data_bytes);
        [DllImport("opus", EntryPoint = "opus_encoder_ctl", CallingConvention = CallingConvention.Cdecl)]
        private static extern int EncoderCtl(IntPtr st, Ctl request, int value);

        /// <summary> Gets the bit rate in kbit/s. </summary>
        public int? BitRate { get; }
        /// <summary> Gets the coding mode of the encoder. </summary>
        public OpusApplication Application { get; }
        /// <summary> Gets the number of channels of this converter. </summary>
        public int InputChannels { get; }
        /// <summary> Gets the milliseconds per frame. </summary>
        public int FrameMilliseconds { get; }

        /// <summary> Gets the bytes per sample. </summary>
        public int SampleSize => (BitsPerSample / 8) * InputChannels;
        /// <summary> Gets the number of samples per frame. </summary>
        public int SamplesPerFrame => SamplingRate / 1000 * FrameMilliseconds;
        /// <summary> Gets the bytes per frame. </summary>
        public int FrameSize => SamplesPerFrame * SampleSize;

        public OpusEncoder(int samplingRate, int channels, int frameMillis, 
            int? bitrate = null, OpusApplication application = OpusApplication.MusicOrMixed)
            : base(samplingRate)
        {
            if (channels != 1 && channels != 2)
                throw new ArgumentOutOfRangeException(nameof(channels));
            if (bitrate != null && (bitrate < 1 || bitrate > AudioClient.MaxBitrate))
                throw new ArgumentOutOfRangeException(nameof(bitrate));

            OpusError error;
            _ptr = CreateEncoder(samplingRate, channels, (int)application, out error);
            if (error != OpusError.OK)
                throw new InvalidOperationException($"Error occured while creating encoder: {error}");


            BitRate = bitrate;
            Application = application;
            InputChannels = channels;
            FrameMilliseconds = frameMillis;

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
                result = Encode(_ptr, inPtr + inputOffset, SamplesPerFrame, output, output.Length);

            if (result < 0)
                throw new Exception(((OpusError)result).ToString());
            return result;
        }

        /// <summary> Gets or sets whether Forward Error Correction is enabled. </summary>
        public void SetForwardErrorCorrection(bool value)
        {
            var result = EncoderCtl(_ptr, Ctl.SetInbandFECRequest, value ? 1 : 0);
            if (result < 0)
                throw new Exception(((OpusError)result).ToString());
        }

        /// <summary> Gets or sets whether Forward Error Correction is enabled. </summary>
        public void SetBitrate(int value)
        {
            var result = EncoderCtl(_ptr, Ctl.SetBitrateRequest, value * 1000);
            if (result < 0)
                throw new Exception(((OpusError)result).ToString());
        }

        protected override void Dispose(bool disposing)
        {
            if (_ptr != IntPtr.Zero)
            {
                DestroyEncoder(_ptr);
                _ptr = IntPtr.Zero;
            }
        }
    }
}
