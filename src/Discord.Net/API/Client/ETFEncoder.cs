using System;
using System.Collections.Generic;
using System.IO;

namespace Discord.API.Client
{
    //TODO: Floats, Atoms, Tuples, Lists, Dictionaries

    public class ETFEncoder
    {
        private const byte SMALL_INTEGER_EXT = 97;
        private const byte INTEGER_EXT = 98;
        private const byte SMALL_BIG_EXT = 110;
        private const byte SMALL_ATOM_EXT = 115;

        private readonly static byte[] nilBytes = new byte[] { SMALL_ATOM_EXT, 3, (byte)'n', (byte)'i', (byte)'l' };
        private readonly static byte[] falseBytes = new byte[] { SMALL_ATOM_EXT, 5, (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e' };
        private readonly static byte[] trueBytes = new byte[] { SMALL_ATOM_EXT, 4, (byte)'t', (byte)'r', (byte)'u', (byte)'e' };

        private void WriteNil(BinaryWriter writer) => Append(writer, nilBytes);
        public void Write(BinaryWriter writer, bool value) => Append(writer, value ? trueBytes : falseBytes);

        public void Write(BinaryWriter writer, byte value) => Write(writer, (ulong)value);
        public void Write(BinaryWriter writer, sbyte value) => Write(writer, (long)value);
        public void Write(BinaryWriter writer, ushort value) => Write(writer, (ulong)value);
        public void Write(BinaryWriter writer, short value) => Write(writer, (long)value);
        public void Write(BinaryWriter writer, uint value) => Write(writer, (ulong)value);
        public void Write(BinaryWriter writer, int value) => Write(writer, (long)value);
        public void Write(BinaryWriter writer, ulong value)
        {
            if (value <= byte.MaxValue)
                Append(writer, new byte[] { SMALL_INTEGER_EXT, (byte)value });
            else if (value <= int.MaxValue)
            {
                Append(writer, new byte[] 
                {
                    INTEGER_EXT,
                    (byte)((value >> 24) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 8) & 0xFF),
                    (byte)(value & 0xFF)
                });
            }
            else
            {
                var buffer = new byte[3 + 8];
                buffer[0] = SMALL_BIG_EXT;
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
                Append(writer, new byte[] { SMALL_INTEGER_EXT, (byte)value });
            else if (value >= int.MinValue && value <= int.MaxValue)
            {
                Append(writer, new byte[]
                {
                    INTEGER_EXT,
                    (byte)((value >> 24) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 8) & 0xFF),
                    (byte)(value & 0xFF)
                });
            }
            else
            {
                var buffer = new byte[3 + 8];
                buffer[0] = SMALL_BIG_EXT;
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
        
        //public void Write(BinaryWriter writer, double value) => Write(writer, (float)value);
        public void Write(BinaryWriter writer, float value)
        {
            throw new NotImplementedException();
        }
        
        public void Write(BinaryWriter writer, byte[] value)
        {
            throw new NotImplementedException();
        }
        public void Write(BinaryWriter writer, string value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(BinaryWriter writer, T value)
            where T : ISerializable
        {
            throw new NotImplementedException();
        }
        public void Write<T>(BinaryWriter writer, IEnumerable<T> value)
            where T : ISerializable
        {
            throw new NotImplementedException();
        }

        private void Append(BinaryWriter writer, byte[] data) => Append(writer, data, data.Length);
        private void Append(BinaryWriter writer, byte[] data, int length)
        {
            throw new NotImplementedException();
        }
    }
}
