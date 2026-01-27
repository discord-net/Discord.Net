using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Discord.Audio
{
    /// <summary>
    ///     Specifies the encryption mode used for voice data.
    /// </summary>
    public enum VoiceEncryptionMode
    {
        /// <summary>
        ///     AES256-GCM encryption with RTP size header.
        ///     Preferred when hardware acceleration is available.
        /// </summary>
        Aes256Gcm,

        /// <summary>
        ///     XChaCha20-Poly1305 encryption with RTP size header.
        ///     Always available, used as fallback.
        /// </summary>
        XChaCha20Poly1305
    }

    /// <summary>
    ///     Provides AEAD encryption functions using libsodium.
    /// </summary>
    public unsafe static class SecretBox
    {
        // XChaCha20-Poly1305 constants
        public const int XChaCha20NonceSize = 24;
        public const int XChaCha20TagSize = 16;

        // AES256-GCM constants
        public const int Aes256GcmNonceSize = 12;
        public const int Aes256GcmTagSize = 16;

        // Nonce counter size (appended to packet)
        public const int NonceCounterSize = 4;

        #region XChaCha20-Poly1305
        [DllImport("libsodium", EntryPoint = "crypto_aead_xchacha20poly1305_ietf_encrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int XChaCha20Encrypt(byte* ciphertext, out ulong ciphertextLength, byte* message, ulong messageLength, byte* ad, ulong adLength, byte* nsec, byte[] nonce, byte[] key);

        [DllImport("libsodium", EntryPoint = "crypto_aead_xchacha20poly1305_ietf_decrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int XChaCha20Decrypt(byte* plaintext, out ulong plaintextLength, byte* nsec, byte* ciphertext, ulong ciphertextLength, byte* ad, ulong adLength, byte[] nonce, byte[] key);
        #endregion

        #region AES256-GCM
        [DllImport("libsodium", EntryPoint = "crypto_aead_aes256gcm_is_available", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Aes256GcmIsAvailable();

        [DllImport("libsodium", EntryPoint = "crypto_aead_aes256gcm_encrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Aes256GcmEncrypt(byte* ciphertext, out ulong ciphertextLength, byte* message, ulong messageLength, byte* ad, ulong adLength, byte* nsec, byte[] nonce, byte[] key);

        [DllImport("libsodium", EntryPoint = "crypto_aead_aes256gcm_decrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Aes256GcmDecrypt(byte* plaintext, out ulong plaintextLength, byte* nsec, byte* ciphertext, ulong ciphertextLength, byte* ad, ulong adLength, byte[] nonce, byte[] key);
        #endregion

        #region Sodium init
        [DllImport("libsodium", EntryPoint = "sodium_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SodiumInit();

        private static bool _initialized;
        private static bool? _aes256GcmAvailable;

        private static void EnsureInitialized()
        {
            if (!_initialized)
            {
                SodiumInit();
                _initialized = true;
            }
        }

        /// <summary>
        ///     Checks if AES256-GCM hardware acceleration is available.
        /// </summary>
        /// <returns><c>true</c> if AES256-GCM is available; otherwise, <c>false</c>.</returns>
        public static bool IsAes256GcmAvailable()
        {
            if (_aes256GcmAvailable.HasValue)
                return _aes256GcmAvailable.Value;

            EnsureInitialized();
            _aes256GcmAvailable = Aes256GcmIsAvailable() == 1;
            return _aes256GcmAvailable.Value;
        }
        #endregion

        /// <summary>
        ///     Encrypts data using the specified encryption mode.
        /// </summary>
        public static int Encrypt(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, byte[] header, byte[] nonce, byte[] key, VoiceEncryptionMode mode)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            fixed (byte* adPtr = header)
            {
                int error;
                ulong cipherLen;

                if (mode == VoiceEncryptionMode.Aes256Gcm)
                {
                    error = Aes256GcmEncrypt(
                        outPtr + outputOffset, out cipherLen,
                        inPtr + inputOffset, (ulong)inputLength,
                        adPtr, (ulong)header.Length,
                        null, nonce, key
                    );
                }
                else
                {
                    error = XChaCha20Encrypt(
                        outPtr + outputOffset, out cipherLen,
                        inPtr + inputOffset, (ulong)inputLength,
                        adPtr, (ulong)header.Length,
                        null, nonce, key
                    );
                }

                if (error != 0)
                    throw new SecurityException($"Sodium AEAD Error: {error}");

                return (int)cipherLen;
            }
        }

        /// <summary>
        ///     Decrypts data using the specified encryption mode.
        /// </summary>
        public static int Decrypt(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, byte[] header, int headerSize, byte[] nonce, byte[] key, VoiceEncryptionMode mode)
        {
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            fixed (byte* adPtr = header)
            {
                int error;
                ulong plainLen;

                if (mode == VoiceEncryptionMode.Aes256Gcm)
                {
                    error = Aes256GcmDecrypt(
                        outPtr + outputOffset, out plainLen,
                        null,
                        inPtr + inputOffset, (ulong)inputLength,
                        adPtr, (ulong)headerSize,
                        nonce, key
                    );
                }
                else
                {
                    error = XChaCha20Decrypt(
                        outPtr + outputOffset, out plainLen,
                        null,
                        inPtr + inputOffset, (ulong)inputLength,
                        adPtr, (ulong)headerSize,
                        nonce, key
                    );
                }

                if (error != 0)
                    throw new SecurityException($"Sodium AEAD Decrypt Error: {error}");

                return (int)plainLen;
            }
        }

        #region Legacy overloads for backwards compatibility
        /// <summary>
        ///     Encrypts data using XChaCha20-Poly1305 (legacy overload).
        /// </summary>
        public static int Encrypt(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, byte[] header, byte[] nonce, byte[] key)
            => Encrypt(input, inputOffset, inputLength, output, outputOffset, header, nonce, key, VoiceEncryptionMode.XChaCha20Poly1305);

        /// <summary>
        ///     Decrypts data using XChaCha20-Poly1305 (legacy overload).
        /// </summary>
        public static int Decrypt(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, byte[] header, int headerSize, byte[] nonce, byte[] key)
            => Decrypt(input, inputOffset, inputLength, output, outputOffset, header, headerSize, nonce, key, VoiceEncryptionMode.XChaCha20Poly1305);
        #endregion

        /// <summary>
        ///     Gets the nonce size for the specified encryption mode.
        /// </summary>
        public static int GetNonceSize(VoiceEncryptionMode mode)
            => mode == VoiceEncryptionMode.Aes256Gcm ? Aes256GcmNonceSize : XChaCha20NonceSize;

        /// <summary>
        ///     Gets the mode string for the specified encryption mode.
        /// </summary>
        public static string GetModeString(VoiceEncryptionMode mode)
            => mode == VoiceEncryptionMode.Aes256Gcm
                ? "aead_aes256_gcm_rtpsize"
                : "aead_xchacha20_poly1305_rtpsize";

        /// <summary>
        ///     Parses a mode string to an encryption mode.
        /// </summary>
        public static VoiceEncryptionMode? ParseMode(string mode)
        {
            return mode switch
            {
                "aead_aes256_gcm_rtpsize" => VoiceEncryptionMode.Aes256Gcm,
                "aead_xchacha20_poly1305_rtpsize" => VoiceEncryptionMode.XChaCha20Poly1305,
                _ => null
            };
        }
    }
}
