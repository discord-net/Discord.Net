using System.Runtime.InteropServices;

namespace Discord.Audio
{
    internal static class Sodium
    {
		[DllImport("lib/libsodium", EntryPoint = "crypto_stream_xor", CallingConvention = CallingConvention.Cdecl)]
		private static extern int StreamXOR(byte[] output, byte[] msg, long msgLength, byte[] nonce, byte[] secret);

		public static int Encrypt(byte[] buffer, int inputLength, byte[] output, byte[] nonce, byte[] secret)
		{
			return StreamXOR(output, buffer, inputLength, nonce, secret);
		}
	}
}
