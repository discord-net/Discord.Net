using System.Runtime.InteropServices;

namespace Discord.Audio
{
    public unsafe static class SecretBox
    {
        [DllImport("libsodium", EntryPoint = "crypto_secretbox_easy", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SecretBoxEasy(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);
        [DllImport("libsodium", EntryPoint = "crypto_secretbox_open_easy", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SecretBoxOpenEasy(byte* output, byte* input, long inputLength, byte[] nonce, byte[] secret);

        public static int Encrypt(byte[] input, int inputOffset, long inputLength, byte[] output, int outputOffset, byte[] nonce, byte[] secret)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
                return SecretBoxEasy(outPtr + outputOffset, inPtr + inputOffset, inputLength, nonce, secret);
        }
        public static int Decrypt(byte[] input, int inputOffset, long inputLength, byte[] output, int outputOffset, byte[] nonce, byte[] secret)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
                return SecretBoxOpenEasy(outPtr + outputOffset, inPtr + inputOffset, inputLength, nonce, secret);
        }
    }
}
