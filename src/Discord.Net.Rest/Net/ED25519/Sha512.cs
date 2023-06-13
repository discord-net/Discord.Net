using System;

namespace Discord.Net.ED25519
{
    internal class Sha512
    {
        private Array8<ulong> _state;
        private readonly byte[] _buffer;
        private ulong _totalBytes;
        public const int BlockSize = 128;
        private static readonly byte[] _padding = new byte[] { 0x80 };

        /// <summary>
        /// Allocation and initialization of the new SHA-512 object.
        /// </summary>
        public Sha512()
        {
            _buffer = new byte[BlockSize];//todo: remove allocation
            Init();
        }

        /// <summary>
        /// Performs an initialization of internal SHA-512 state.
        /// </summary>
        public void Init()
        {
            Sha512Internal.Sha512Init(out _state);
            _totalBytes = 0;
        }

        /// <summary>
        /// Updates internal state with data from the provided array segment.
        /// </summary>
        /// <param name="data">Array segment</param>
        public void Update(ArraySegment<byte> data)
        {
            Update(data.Array, data.Offset, data.Count);
        }

        /// <summary>
        /// Updates internal state with data from the provided array.
        /// </summary>
        /// <param name="data">Array of bytes</param>
        /// <param name="index">Offset of byte sequence</param>
        /// <param name="length">Sequence length</param>
        public void Update(byte[] data, int index, int length)
        {

            Array16<ulong> block;
            int bytesInBuffer = (int)_totalBytes & (BlockSize - 1);
            _totalBytes += (uint)length;

            if (_totalBytes >= ulong.MaxValue / 8)
                throw new InvalidOperationException("Too much data");
            // Fill existing buffer
            if (bytesInBuffer != 0)
            {
                var toCopy = Math.Min(BlockSize - bytesInBuffer, length);
                Buffer.BlockCopy(data, index, _buffer, bytesInBuffer, toCopy);
                index += toCopy;
                length -= toCopy;
                bytesInBuffer += toCopy;
                if (bytesInBuffer == BlockSize)
                {
                    ByteIntegerConverter.Array16LoadBigEndian64(out block, _buffer, 0);
                    Sha512Internal.Core(out _state, ref _state, ref block);
                    CryptoBytes.InternalWipe(_buffer, 0, _buffer.Length);
                    bytesInBuffer = 0;
                }
            }
            // Hash complete blocks without copying
            while (length >= BlockSize)
            {
                ByteIntegerConverter.Array16LoadBigEndian64(out block, data, index);
                Sha512Internal.Core(out _state, ref _state, ref block);
                index += BlockSize;
                length -= BlockSize;
            }
            // Copy remainder into buffer
            if (length > 0)
            {
                Buffer.BlockCopy(data, index, _buffer, bytesInBuffer, length);
            }
        }

        /// <summary>
        /// Finalizes SHA-512 hashing
        /// </summary>
        /// <param name="output">Output buffer</param>
        public void Finalize(ArraySegment<byte> output)
        {
            Preconditions.NotNull(output.Array, nameof(output));
            if (output.Count != 64)
                throw new ArgumentException("Output should be 64 in length");

            Update(_padding, 0, _padding.Length);
            Array16<ulong> block;
            ByteIntegerConverter.Array16LoadBigEndian64(out block, _buffer, 0);
            CryptoBytes.InternalWipe(_buffer, 0, _buffer.Length);
            int bytesInBuffer = (int)_totalBytes & (BlockSize - 1);
            if (bytesInBuffer > BlockSize - 16)
            {
                Sha512Internal.Core(out _state, ref _state, ref block);
                block = default(Array16<ulong>);
            }
            block.x15 = (_totalBytes - 1) * 8;
            Sha512Internal.Core(out _state, ref _state, ref block);

            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 0, _state.x0);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 8, _state.x1);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 16, _state.x2);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 24, _state.x3);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 32, _state.x4);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 40, _state.x5);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 48, _state.x6);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 56, _state.x7);
            _state = default(Array8<ulong>);
        }

        /// <summary>
        /// Finalizes SHA-512 hashing.
        /// </summary>
        /// <returns>Hash bytes</returns>
        public byte[] Finalize()
        {
            var result = new byte[64];
            Finalize(new ArraySegment<byte>(result));
            return result;
        }

        /// <summary>
        /// Calculates SHA-512 hash value for the given bytes array.
        /// </summary>
        /// <param name="data">Data bytes array</param>
        /// <returns>Hash bytes</returns>
        public static byte[] Hash(byte[] data)
        {
            return Hash(data, 0, data.Length);
        }

        /// <summary>
        /// Calculates SHA-512 hash value for the given bytes array.
        /// </summary>
        /// <param name="data">Data bytes array</param>
        /// <param name="index">Offset of byte sequence</param>
        /// <param name="length">Sequence length</param>
        /// <returns>Hash bytes</returns>
        public static byte[] Hash(byte[] data, int index, int length)
        {
            var hasher = new Sha512();
            hasher.Update(data, index, length);
            return hasher.Finalize();
        }
    }
}
