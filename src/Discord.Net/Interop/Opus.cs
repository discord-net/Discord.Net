using System;
using System.Runtime.InteropServices;

namespace Discord.Interop
{
	internal unsafe static class Opus
	{
		[DllImport("lib/opus", EntryPoint = "opus_encoder_create", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr CreateEncoder(int Fs, int channels, int application, out Error error);
		[DllImport("lib/opus", EntryPoint = "opus_encoder_destroy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void DestroyEncoder(IntPtr encoder);
		[DllImport("lib/opus", EntryPoint = "opus_encode", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Encode(IntPtr st, byte* pcm, int frame_size, byte[] data, int max_data_bytes);

		[DllImport("lib/opus", EntryPoint = "opus_decoder_create", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr CreateDecoder(int Fs, int channels, out Error error);
		[DllImport("lib/opus", EntryPoint = "opus_decoder_destroy", CallingConvention = CallingConvention.Cdecl)]
		public static extern void DestroyDecoder(IntPtr decoder);
		[DllImport("lib/opus", EntryPoint = "opus_decode", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Decode(IntPtr st, byte* data, int len, byte[] pcm, int frame_size, int decode_fec);

		[DllImport("lib/opus", EntryPoint = "opus_encoder_ctl", CallingConvention = CallingConvention.Cdecl)]
		public static extern int EncoderCtl(IntPtr st, Ctl request, int value);

		public enum Ctl : int
		{
			SetBitrateRequest = 4002,
			GetBitrateRequest = 4003,
			SetInbandFECRequest = 4012,
			GetInbandFECRequest = 4013
		}

		/// <summary>Supported coding modes.</summary>
		public enum Application : int
		{
			/// <summary>
			/// Gives best quality at a given bitrate for voice signals. It enhances the input signal by high-pass filtering and emphasizing formants and harmonics. 
			/// Optionally it includes in-band forward error correction to protect against packet loss. Use this mode for typical VoIP applications. 
			/// Because of the enhancement, even at high bitrates the output may sound different from the input.
			/// </summary>
			Voip = 2048,
			/// <summary>
			/// Gives best quality at a given bitrate for most non-voice signals like music. 
			/// Use this mode for music and mixed (music/voice) content, broadcast, and applications requiring less than 15 ms of coding delay.
			/// </summary>
			Audio = 2049,
			/// <summary> Low-delay mode that disables the speech-optimized mode in exchange for slightly reduced delay.  </summary>
			Restricted_LowLatency = 2051
		}

		public enum Error : int
		{
			/// <summary>  No error. </summary>
			OK = 0,
			/// <summary> One or more invalid/out of range arguments. </summary>
			BadArg = -1,
			/// <summary> The mode struct passed is invalid. </summary>
			BufferToSmall = -2,
			/// <summary> An internal error was detected. </summary>
			InternalError = -3,
			/// <summary> The compressed data passed is corrupted. </summary>
			InvalidPacket = -4,
			/// <summary> Invalid/unsupported request number. </summary>
			Unimplemented = -5,
			/// <summary> An encoder or decoder structure is invalid or already freed.  </summary>
			InvalidState = -6,
			/// <summary> Memory allocation has failed. </summary>
			AllocFail = -7
		}
	}
}
