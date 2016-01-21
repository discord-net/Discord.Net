using System;
using System.Collections.Generic;
using System.IO;

namespace Discord.ETF
{
    //TODO: Floats, Atoms, Tuples, Lists, Dictionaries

    public unsafe class ETFEncoder
    {
        private readonly static byte[] _nilBytes = new byte[] { (byte)ETFType.SMALL_ATOM_EXT, 3, (byte)'n', (byte)'i', (byte)'l' };
        private readonly static byte[] _falseBytes = new byte[] { (byte)ETFType.SMALL_ATOM_EXT, 5, (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e' };
        private readonly static byte[] _trueBytes = new byte[] { (byte)ETFType.SMALL_ATOM_EXT, 4, (byte)'t', (byte)'r', (byte)'u', (byte)'e' };
        private byte[] _writeBuffer;

        public ETFEncoder()
        {
            _writeBuffer = new byte[11];
        }

        private void WriteNil(BinaryWriter writer) => Append(writer, _nilBytes);
        public void Write(BinaryWriter writer, bool value) => Append(writer, value ? _trueBytes : _falseBytes);

        public void Write(BinaryWriter writer, byte value) => Write(writer, (ulong)value);
        public void Write(BinaryWriter writer, sbyte value) => Write(writer, (long)value);
        public void Write(BinaryWriter writer, ushort value) => Write(writer, (ulong)value);
        public void Write(BinaryWriter writer, short value) => Write(writer, (long)value);
        public void Write(BinaryWriter writer, uint value) => Write(writer, (ulong)value);
        public void Write(BinaryWriter writer, int value) => Write(writer, (long)value);
        public void Write(BinaryWriter writer, ulong value)
        {
            if (value <= byte.MaxValue)
                Append(writer, new byte[] { (byte)ETFType.SMALL_INTEGER_EXT, (byte)value });
            else if (value <= int.MaxValue)
            {
                Append(writer, (byte)ETFType.INTEGER_EXT,
                    (byte)((value >> 24) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 8) & 0xFF),
                    (byte)(value & 0xFF));
            }
            else
            {
                var buffer = new byte[3 + 8];
                buffer[0] = (byte)ETFType.SMALL_BIG_EXT;
                //buffer[1] = 0; //Always positive

                byte bytes = 0;
                while (value > 0)
                {
                    buffer[3 + bytes] = (byte)(value & 0xFF);
                    value >>= 8;
                    bytes++;
                }
                buffer[1] = bytes; //Encoded bytes

                Append(writer, buffer, 3 + bytes);
            }
        }
        public void Write(BinaryWriter writer, long value)
        {
            if (value >= byte.MinValue && value <= byte.MaxValue)
            {
                Append(writer, (byte)ETFType.SMALL_INTEGER_EXT,
                    (byte)value);
            }
            else if (value >= int.MinValue && value <= int.MaxValue)
            {
                Append(writer, (byte)ETFType.INTEGER_EXT,
                    (byte)((value >> 24) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 8) & 0xFF),
                    (byte)(value & 0xFF));
            }
            else
            {
                var buffer = new byte[3 + 8];
                buffer[0] = (byte)ETFType.SMALL_BIG_EXT;
                if (value < 0)
                {
                    buffer[2] = 1; //Is negative
                    value = -value;
                }

                byte bytes = 0;
                while (value > 0)
                {
                    buffer[3 + bytes] = (byte)(value & 0xFF);
                    value >>= 8;
                    bytes++;
                }
                buffer[1] = bytes; //Encoded bytes

                Append(writer, buffer, 3 + bytes);
            }
        }
        
        public void Write(BinaryWriter writer, float value) => Write(writer, (double)value);
        public void Write(BinaryWriter writer, double value)
        {
            ulong value2 = *(ulong*)&value;
            Append(writer, (byte)ETFType.NEW_FLOAT_EXT,
                (byte)((value2 >> 56) & 0xFF),
                (byte)((value2 >> 48) & 0xFF),
                (byte)((value2 >> 40) & 0xFF),
                (byte)((value2 >> 32) & 0xFF),
                (byte)((value2 >> 24) & 0xFF),
                (byte)((value2 >> 16) & 0xFF),
                (byte)((value2 >> 8) & 0xFF),
                (byte)(value2 & 0xFF));
        }
        
        public void Write(BinaryWriter writer, byte[] value)
        {
            int count = value.Length;
            Append(writer, (byte)ETFType.BINARY_EXT,
                (byte)((count >> 24) & 0xFF),
                (byte)((count >> 16) & 0xFF),
                (byte)((count >> 8) & 0xFF),
                (byte)(count & 0xFF));
            Append(writer, value);
        }
        public void Write(BinaryWriter writer, string value)
        {
            throw new NotImplementedException();
        }

        /*public void Write<T>(BinaryWriter writer, T value)
            where T : ISerializable
        {
            throw new NotImplementedException();
        }
        public void Write<T>(BinaryWriter writer, IEnumerable<T> value)
            where T : ISerializable
        {
            throw new NotImplementedException();
        }*/

        private void Append(BinaryWriter writer, params byte[] data) => Append(writer, data, data.Length);
        private void Append(BinaryWriter writer, byte[] data, int length)
        {
            throw new NotImplementedException();
        }
    }
}
