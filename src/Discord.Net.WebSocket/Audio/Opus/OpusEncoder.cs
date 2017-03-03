using System;
using System.Runtime.InteropServices;

namespace Discord.Audio
{
    internal unsafe class OpusEncoder : OpusConverter
    {
        [DllImport("opus", EntryPoint = "opus_encoder_create", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateEncoder(int Fs, int channels, int application, out OpusError error);
        [DllImport("opus", EntryPoint = "opus_encoder_destroy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DestroyEncoder(IntPtr encoder);
        [DllImport("opus", EntryPoint = "opus_encode", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Encode(IntPtr st, byte* pcm, int frame_size, byte* data, int max_data_bytes);
        [DllImport("opus", EntryPoint = "opus_encoder_ctl", CallingConvention = CallingConvention.Cdecl)]
        private static extern int EncoderCtl(IntPtr st, OpusCtl request, int value);
        
        /// <summary> Gets the coding mode of the encoder. </summary>
        public AudioApplication Application { get; }
        public int BitRate { get;}

        public OpusEncoder(int samplingRate, int channels, int bitrate, AudioApplication application)
            : base(samplingRate, channels)
        {
            if (bitrate < 1 || bitrate > DiscordVoiceAPIClient.MaxBitrate)
                throw new ArgumentOutOfRangeException(nameof(bitrate));

            Application = application;
            BitRate = bitrate;

            OpusApplication opusApplication;
            OpusSignal opusSignal;
            switch (application)
            {
                case AudioApplication.Mixed:
                    opusApplication = OpusApplication.MusicOrMixed;
                    opusSignal = OpusSignal.Auto;
                    break;
                case AudioApplication.Music:
                    opusApplication = OpusApplication.MusicOrMixed;
                    opusSignal = OpusSignal.Music;
                    break;
                case AudioApplication.Voice:
                    opusApplication = OpusApplication.Voice;
                    opusSignal = OpusSignal.Voice;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(application));
            }

            OpusError error;
            _ptr = CreateEncoder(samplingRate, channels, (int)opusApplication, out error);
            if (error != OpusError.OK)
                throw new Exception($"Opus Error: {error}");

            var result = EncoderCtl(_ptr, OpusCtl.SetSignal, (int)opusSignal);
            if (result < 0)
                throw new Exception($"Opus Error: {(OpusError)result}");

            result = EncoderCtl(_ptr, OpusCtl.SetPacketLossPercent, 5); //%%
            if (result < 0)
                throw new Exception($"Opus Error: {(OpusError)result}");

            result = EncoderCtl(_ptr, OpusCtl.SetInbandFEC, 1); //True
            if (result < 0)
                throw new Exception($"Opus Error: {(OpusError)result}");

            result = EncoderCtl(_ptr, OpusCtl.SetBitrate, bitrate);
            if (result < 0)
                throw new Exception($"Opus Error: {(OpusError)result}");

            /*if (application == AudioApplication.Music)
            {
                result = EncoderCtl(_ptr, OpusCtl.SetBandwidth, 1105);
                if (result < 0)
                    throw new Exception($"Opus Error: {(OpusError)result}");
            }*/
        }

        /// <summary> Produces Opus encoded audio from PCM samples. </summary>
        /// <param name="input">PCM samples to encode.</param>
        /// <param name="output">Buffer to store the encoded frame.</param>
        /// <returns>Length of the frame contained in outputBuffer.</returns>
        public unsafe int EncodeFrame(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
            int result = 0;
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
                result = Encode(_ptr, inPtr + inputOffset, inputCount / SampleSize, outPtr + outputOffset, output.Length - outputOffset);

            if (result < 0)
                throw new Exception($"Opus Error: {(OpusError)result}");
            return result;
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
