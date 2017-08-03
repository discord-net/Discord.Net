using System;
using System.Runtime.InteropServices;

namespace Discord.Audio
{
    public unsafe static class SecretBox
    {
        [DllImport("libsodium", EntryPoint = "crypto_secretbox_easy", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SecretBoxEasy(byte* output, byte* input, long inputLength, byte* nonce, byte* secret);
        [DllImport("libsodium", EntryPoint = "crypto_secretbox_open_easy", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SecretBoxOpenEasy(byte* output, byte* input, long inputLength, byte* nonce, byte* secret);

        public static int Encrypt(byte* inPtr, int inputOffset, int inputLength, byte* outPtr, int outputOffset, byte* nonce, byte* secret)
        {
            int error = SecretBoxEasy(outPtr + outputOffset, inPtr + inputOffset, inputLength, nonce, secret);
            if (error != 0)
                throw new Exception($"Sodium Error: {error}");
            return inputLength + 16;
        }
        public static int Decrypt(byte* inPtr, int inputOffset, int inputLength, byte* outPtr, int outputOffset, byte* nonce, byte* secret)
        {
            int error = SecretBoxOpenEasy(outPtr + outputOffset, inPtr + inputOffset, inputLength, nonce, secret);
            if (error != 0)
                throw new Exception($"Sodium Error: {error}");
            return inputLength - 16;
        }
    }
}
