using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.ED25519
{
    internal static class Ed25519
    {
        /// <summary>
        /// Public Keys are 32 byte values. All possible values of this size a valid.
        /// </summary>
        public const int PublicKeySize = 32;
        /// <summary>
        /// Signatures are 64 byte values
        /// </summary>
        public const int SignatureSize = 64;
        /// <summary>
        /// Private key seeds are 32 byte arbitrary values. This is the form that should be generated and stored.
        /// </summary>
        public const int PrivateKeySeedSize = 32;
        /// <summary>
        /// A 64 byte expanded form of private key. This form is used internally to improve performance
        /// </summary>
        public const int ExpandedPrivateKeySize = 32 * 2;

        /// <summary>
        /// Verify Ed25519 signature
        /// </summary>
        /// <param name="signature">Signature bytes</param>
        /// <param name="message">Message</param>
        /// <param name="publicKey">Public key</param>
        /// <returns>True if signature is valid, false if it's not</returns>
        public static bool Verify(ArraySegment<byte> signature, ArraySegment<byte> message, ArraySegment<byte> publicKey)
        {
            if (signature.Count != SignatureSize)
                throw new ArgumentException($"Sizeof signature doesnt match defined size of {SignatureSize}");

            if (publicKey.Count != PublicKeySize)
                throw new ArgumentException($"Sizeof public key doesnt match defined size of {PublicKeySize}");

            return Ed25519Operations.crypto_sign_verify(signature.Array, signature.Offset, message.Array, message.Offset, message.Count, publicKey.Array, publicKey.Offset);
        }

        /// <summary>
        /// Verify Ed25519 signature
        /// </summary>
        /// <param name="signature">Signature bytes</param>
        /// <param name="message">Message</param>
        /// <param name="publicKey">Public key</param>
        /// <returns>True if signature is valid, false if it's not</returns>
        public static bool Verify(byte[] signature, byte[] message, byte[] publicKey)
        {
            Preconditions.NotNull(signature, nameof(signature));
            Preconditions.NotNull(message, nameof(message));
            Preconditions.NotNull(publicKey, nameof(publicKey));
            if (signature.Length != SignatureSize)
                throw new ArgumentException($"Sizeof signature doesnt match defined size of {SignatureSize}");

            if (publicKey.Length != PublicKeySize)
                throw new ArgumentException($"Sizeof public key doesnt match defined size of {PublicKeySize}");

            return Ed25519Operations.crypto_sign_verify(signature, 0, message, 0, message.Length, publicKey, 0);
        }
    }
}
