using System;
using System.Runtime.InteropServices;

namespace Discord.Audio
{
    public unsafe static class SecretBox
    {
        // from crypto_secretbox.h and crypto_secretbox_xsalsa20poly1305.h
        private const uint crypto_secretbox_KEYBYTES = 32U;
        private const uint crypto_secretbox_NONCEBYTES = 24U;
        private const uint crypto_secretbox_MACBYTES = 16U;

        [DllImport("libsodium", CallingConvention = CallingConvention.Cdecl)]
        private static extern int crypto_secretbox_easy(
            [Out] byte[] c, // ciphertext
            [In] byte[] m, // message
            [MarshalAs(UnmanagedType.U8)] ulong mlen, // length of message
            [In] byte[] n, // nonce
            [In] byte[] k // key
            );

        [DllImport("libsodium", CallingConvention = CallingConvention.Cdecl)]
        private static extern int crypto_secretbox_open_easy(
            [Out] byte[] m, // message
            [In] byte[] c, // ciphertext
            [MarshalAs(UnmanagedType.U8)] ulong clen, // length of ciphertext
            [In] byte[] n, // nonce
            [In] byte[] k // key
            );

        // Both of the above functions return 0 on success, -1 on error

        public static void Encrypt(byte[] input, int inputOffset, ulong inputLength, byte[] output, int outputOffset, byte[] nonce, byte[] secret)
        {
            byte[] _input = new byte[inputLength];
            Buffer.BlockCopy(input, inputOffset, _input, 0, (int)inputLength); // TODO: this will probably cause an overflow or throw an exception...

            byte[] _output = new byte[crypto_secretbox_MACBYTES + inputLength];
            int result = crypto_secretbox_easy(_output, _input, inputLength, nonce, secret);

            if (result == -1)
                throw new InvalidOperationException("Failed to encrypt the provided payload");

            Buffer.BlockCopy(_output, 0, output, outputOffset, _output.Length);
        }

        public static void Decrypt(byte[] input, int inputOffset, ulong inputLength, byte[] output, int outputOffset, byte[] nonce, byte[] secret)
        {
            if (inputLength < crypto_secretbox_MACBYTES)
                throw new ArgumentException($"the length of the input arary must be greater than {crypto_secretbox_MACBYTES} bytes", "inputLength");

            byte[] _input = new byte[inputLength];
            Buffer.BlockCopy(input, inputOffset, _input, 0, (int)inputLength); // TODO: this will also probably overflow or throw an exception

            byte[] _output = new byte[inputLength - crypto_secretbox_MACBYTES];
            int result = crypto_secretbox_open_easy(_output, _input, inputLength, nonce, secret);

            if (result == -1)
                throw new InvalidOperationException("Failed to verify the decrypted payload");

            Buffer.BlockCopy(_output, 0, output, outputOffset, _output.Length);
        }

        /* I kept the old code here to refer to in case it is needed
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
        */
    }
}
