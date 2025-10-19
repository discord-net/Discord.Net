using System.Runtime.InteropServices;
using System.Security;

namespace Discord.Audio
{
    public unsafe static class SecretBox
    {
        [DllImport("libsodium", EntryPoint = "crypto_aead_xchacha20poly1305_ietf_encrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Encrypt(byte* ciphertext, out ulong ciphertextLength, byte* message, ulong messageLength, byte* ad, ulong adLength, byte* nsec, byte[] nonce, byte[] key);

        [DllImport("libsodium", EntryPoint = "crypto_aead_xchacha20poly1305_ietf_decrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Decrypt(byte* plaintext, out ulong plaintextLength, byte* nsec, byte* ciphertext, ulong ciphertextLength, byte* ad, ulong adLength, byte[] nonce, byte[] key);

        public static int Encrypt(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, byte[] header, byte[] nonce, byte[] key)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            fixed (byte* adPtr = header)
            {
                int error = Encrypt(
                    outPtr + outputOffset, out ulong cipherLen,
                    inPtr + inputOffset, (ulong)inputLength,
                    adPtr, (ulong)header.Length,
                    null, nonce, key
                );

                if (error != 0)
                    throw new SecurityException($"Sodium AEAD Error: {error}");

                return (int)cipherLen;
            }
        }

        public static int Decrypt(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, byte[] header, int headerSize, byte[] nonce, byte[] key)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            fixed (byte* adPtr = header)
            {
                int error = Decrypt(
                    outPtr + outputOffset, out ulong plainLen,
                    null,
                    inPtr + inputOffset, (ulong)inputLength,
                    adPtr, (ulong)headerSize,
                    nonce, key
                );

                if (error != 0)
                    throw new SecurityException($"Sodium AEAD Decrypt Error: {error}");

                return (int)plainLen;
            }
        }
    }
}
