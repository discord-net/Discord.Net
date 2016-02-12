using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;

namespace Discord.ETF
{
    public class ETFReader
    {
        /*
        private readonly Stream _stream;
        private readonly byte[] _buffer;
        private readonly bool _leaveOpen;
        private readonly Encoding _encoding;
        private readonly ConcurrentDictionary<Type, Delegate> _serializers, _indirectSerializers;

        private void ReadNil(bool allow)
        {
            if (!allow) throw new InvalidDataException();
            _stream.Read(_buffer, 0, 3);
            if (_buffer[0] != 'n' || _buffer[1] != 'i' || _buffer[2] != 'l')
                throw new InvalidDataException();
        }
        private void ReadTrue()
        {
            _stream.Read(_buffer, 0, 4);
            if (_buffer[0] != 't' || _buffer[1] != 'r' || _buffer[2] != 'u' || _buffer[3] != 'e')
                throw new InvalidDataException();
        }
        private void ReadFalse()
        {
            _stream.Read(_buffer, 0, 5);
            if (_buffer[0] != 'f' || _buffer[1] != 'a' || _buffer[2] != 'l' || _buffer[3] != 's' || _buffer[4] != 'e')
                throw new InvalidDataException();
        }

        public bool? ReadBool(bool allowNil)
        {
            _stream.Read(_buffer, 0, 2);
            switch ((ETFType)_buffer[0])
            {
                case ETFType.SMALL_ATOM_EXT:
                    switch (_buffer[1]) //Length
                    {
                        case 3:
                            ReadNil(allowNil);
                            return null;
                        case 4:
                            ReadTrue();
                            return true;
                        case 5:
                            ReadFalse();
                            return false;
                    }
                    break;
            }
            throw new InvalidDataException();
        }

        public long? ReadInteger(bool allowNil)
        {
            _stream.Read(_buffer, 0, 1);
            ETFType type = (ETFType)reader.ReadByte();
            switch (type)
            {
                case ETFType.SMALL_ATOM_EXT:
                    ReadNil(allowNil);
                    return null;
                case ETFType.SMALL_INTEGER_EXT:
                    _stream.Read(_buffer, 0, 1);
                    return (_buffer[0] << 24) | (_buffer[1] << 16) |
                        (_buffer[2] << 8) | (_buffer[3]);
                case ETFType.INTEGER_EXT:
                    _stream.Read(_buffer, 0, 4);
                    return ??;
                case ETFType.SMALL_BIG_EXT:
                    return ??;
            }
            throw new InvalidDataException();
        }

        public void Write(sbyte value) => Write((long)value);
        public void Write(byte value) => Write((ulong)value);
        public void Write(short value) => Write((long)value);
        public void Write(ushort value) => Write((ulong)value);
        public void Write(int value) => Write((long)value);
        public void Write(uint value) => Write((ulong)value);
        public void Write(long value)
        {
            if (value >= byte.MinValue && value <= byte.MaxValue)
            {
                _buffer[0] = (byte)ETFType.SMALL_INTEGER_EXT;
                _buffer[1] = (byte)value;
                _stream.Write(_buffer, 0, 2);
            }
            else if (value >= int.MinValue && value <= int.MaxValue)
            {
                _buffer[0] = (byte)ETFType.INTEGER_EXT;
                _buffer[1] = (byte)(value >> 24);
                _buffer[2] = (byte)(value >> 16);
                _buffer[3] = (byte)(value >> 8);
                _buffer[4] = (byte)value;
                _stream.Write(_buffer, 0, 5);
            }
            else
            {
                _buffer[0] = (byte)ETFType.SMALL_BIG_EXT;
                if (value < 0)
                {
                    _buffer[2] = 1; //Is negative
                    value = -value;
                }

                byte bytes = 0;
                while (value > 0)
                    _buffer[3 + bytes++] = (byte)(value >>= 8);
                _buffer[1] = bytes; //Encoded bytes

                _stream.Write(_buffer, 0, 3 + bytes);
            }
        }
        public void Write(ulong value)
        {
            if (value <= byte.MaxValue)
            {
                _buffer[0] = (byte)ETFType.SMALL_INTEGER_EXT;
                _buffer[1] = (byte)value;
                _stream.Write(_buffer, 0, 2);
            }
            else if (value <= int.MaxValue)
            {
                _buffer[0] = (byte)ETFType.INTEGER_EXT;
                _buffer[1] = (byte)(value >> 24);
                _buffer[2] = (byte)(value >> 16);
                _buffer[3] = (byte)(value >> 8);
                _buffer[4] = (byte)value;
                _stream.Write(_buffer, 0, 5);
            }
            else
            {
                _buffer[0] = (byte)ETFType.SMALL_BIG_EXT;
                _buffer[2] = 0; //Always positive

                byte bytes = 0;
                while (value > 0)
                    _buffer[3 + bytes++] = (byte)(value >>= 8);
                _buffer[1] = bytes; //Encoded bytes

                _stream.Write(_buffer, 0, 3 + bytes);
            }
        }

        public void Write(float value) => Write((double)value);
        public unsafe void Write(double value)
        {
            ulong value2 = *(ulong*)&value;
            _buffer[0] = (byte)ETFType.NEW_FLOAT_EXT;
            _buffer[1] = (byte)(value2 >> 56);
            _buffer[2] = (byte)(value2 >> 48);
            _buffer[3] = (byte)(value2 >> 40);
            _buffer[4] = (byte)(value2 >> 32);
            _buffer[5] = (byte)(value2 >> 24);
            _buffer[6] = (byte)(value2 >> 16);
            _buffer[7] = (byte)(value2 >> 8);
            _buffer[8] = (byte)value2;
            _stream.Write(_buffer, 0, 9);
        }

        public void Write(DateTime value) => Write((ulong)((value.Ticks - _epochTime.Ticks) / TimeSpan.TicksPerSecond));
*/
    }
}