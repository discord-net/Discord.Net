//using Newtonsoft.Json;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Text;

//namespace Discord.ETF
//{
//    public class ETFReader : IDisposable
//    {
//        private static readonly ConcurrentDictionary<Type, Delegate> _deserializers = new ConcurrentDictionary<Type, Delegate>();
//        private static readonly Dictionary<Type, MethodInfo> _readMethods = GetPrimitiveReadMethods();

//        private readonly Stream _stream;
//        private readonly byte[] _buffer;
//        private readonly bool _leaveOpen;
//        private readonly Encoding _encoding;
        
//        public ETFReader(Stream stream, bool leaveOpen = false)
//        {
//            if (stream == null) throw new ArgumentNullException(nameof(stream));

//            _stream = stream;
//            _leaveOpen = leaveOpen;
//            _buffer = new byte[11];
//            _encoding = Encoding.UTF8;
//        }
        
//        public bool ReadBool()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            if (type == ETFType.SMALL_ATOM_EXT)
//            {
//                _stream.Read(_buffer, 0, 1);
//                switch (_buffer[0]) //Length
//                {
//                    case 4:
//                        ReadTrue();
//                        return true;
//                    case 5:
//                        ReadFalse();
//                        return false;
//                }
//            }
//            throw new InvalidDataException();
//        }
//        private void ReadTrue()
//        {
//            _stream.Read(_buffer, 0, 4);
//            if (_buffer[0] != 't' || _buffer[1] != 'r' || _buffer[2] != 'u' || _buffer[3] != 'e')
//                throw new InvalidDataException();
//        }
//        private void ReadFalse()
//        {
//            _stream.Read(_buffer, 0, 5);
//            if (_buffer[0] != 'f' || _buffer[1] != 'a' || _buffer[2] != 'l' || _buffer[3] != 's' || _buffer[4] != 'e')
//                throw new InvalidDataException();
//        }

//        public int ReadSByte()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            return (sbyte)ReadLongInternal(type);
//        }
//        public uint ReadByte()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            return (byte)ReadLongInternal(type);
//        }
//        public int ReadShort()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            return (short)ReadLongInternal(type);
//        }
//        public uint ReadUShort()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            return (ushort)ReadLongInternal(type);
//        }
//        public int ReadInt()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            return (int)ReadLongInternal(type);
//        }
//        public uint ReadUInt()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            return (uint)ReadLongInternal(type);
//        }
//        public long ReadLong()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            return ReadLongInternal(type);
//        }
//        public ulong ReadULong()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            return (ulong)ReadLongInternal(type);
//        }
//        public long ReadLongInternal(ETFType type)
//        {
//            switch (type)
//            {
//                case ETFType.SMALL_INTEGER_EXT:
//                    _stream.Read(_buffer, 0, 1);
//                    return _buffer[0];
//                case ETFType.INTEGER_EXT:
//                    _stream.Read(_buffer, 0, 4);
//                    return (_buffer[0] << 24) | (_buffer[1] << 16) | (_buffer[2] << 8) | (_buffer[3]);
//                case ETFType.SMALL_BIG_EXT:
//                    _stream.Read(_buffer, 0, 2);
//                    bool isPositive = _buffer[0] == 0;
//                    byte count = _buffer[1];

//                    int shiftValue = (count - 1) * 8;
//                    ulong value = 0;
//                    _stream.Read(_buffer, 0, count);
//                    for (int i = 0; i < count; i++, shiftValue -= 8)
//                        value = value + _buffer[i] << shiftValue;
//                    if (!isPositive)
//                        return -(long)value;
//                    else
//                        return (long)value;
//            }
//            throw new InvalidDataException();
//        }

//        public float ReadSingle()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            return (float)ReadDoubleInternal(type);
//        }
//        public double ReadDouble()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            return ReadDoubleInternal(type);
//        }
//        public double ReadDoubleInternal(ETFType type)
//        {
//            throw new NotImplementedException();
//        }

//        public bool? ReadNullableBool()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            if (type == ETFType.SMALL_ATOM_EXT)
//            {
//                _stream.Read(_buffer, 0, 1);
//                switch (_buffer[0]) //Length
//                {
//                    case 3:
//                        if (ReadNil())
//                            return null;
//                        break;
//                    case 4:
//                        ReadTrue();
//                        return true;
//                    case 5:
//                        ReadFalse();
//                        return false;
//                }
//            }
//            throw new InvalidDataException();
//        }
//        public int? ReadNullableSByte()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            if (type == ETFType.SMALL_ATOM_EXT && ReadNil()) return null;
//            return (sbyte)ReadLongInternal(type);
//        }
//        public uint? ReadNullableByte()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            if (type == ETFType.SMALL_ATOM_EXT && ReadNil()) return null;
//            return (byte)ReadLongInternal(type);
//        }
//        public int? ReadNullableShort()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            if (type == ETFType.SMALL_ATOM_EXT && ReadNil()) return null;
//            return (short)ReadLongInternal(type);
//        }
//        public uint? ReadNullableUShort()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            if (type == ETFType.SMALL_ATOM_EXT && ReadNil()) return null;
//            return (ushort)ReadLongInternal(type);
//        }
//        public int? ReadNullableInt()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            if (type == ETFType.SMALL_ATOM_EXT && ReadNil()) return null;
//            return (int)ReadLongInternal(type);
//        }
//        public uint? ReadNullableUInt()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            if (type == ETFType.SMALL_ATOM_EXT && ReadNil()) return null;
//            return (uint)ReadLongInternal(type);
//        }
//        public long? ReadNullableLong()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            if (type == ETFType.SMALL_ATOM_EXT && ReadNil()) return null;
//            return ReadLongInternal(type);
//        }
//        public ulong? ReadNullableULong()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            if (type == ETFType.SMALL_ATOM_EXT && ReadNil()) return null;
//            return (ulong)ReadLongInternal(type);
//        }
//        public float? ReadNullableSingle()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            if (type == ETFType.SMALL_ATOM_EXT && ReadNil()) return null;
//            return (float)ReadDoubleInternal(type);
//        }
//        public double? ReadNullableDouble()
//        {
//            _stream.Read(_buffer, 0, 1);
//            ETFType type = (ETFType)_buffer[0];
//            if (type == ETFType.SMALL_ATOM_EXT && ReadNil()) return null;
//            return ReadDoubleInternal(type);
//        }

//        public string ReadString()
//        {
//            throw new NotImplementedException();
//        }
//        public byte[] ReadByteArray()
//        {
//            throw new NotImplementedException();
//        }

//        public T Read<T>()
//            where T : new()
//        {
//            var type = typeof(T);
//            var typeInfo = type.GetTypeInfo();
//            var action = _deserializers.GetOrAdd(type, _ => CreateDeserializer<T>(type, typeInfo)) as Func<ETFReader, T>;
//            return action(this);
//        }
//        /*public void Read<T, U>()
//            where T : Nullable<T>
//            where U : struct, new()
//        {
//        }*/
//        public T[] ReadArray<T>()
//        {
//            throw new NotImplementedException();
//        }
//        public IDictionary<TKey, TValue> ReadDictionary<TKey, TValue>()
//        {
//            throw new NotImplementedException();
//        }
//        /*public object Read(object obj)
//        {
//            throw new NotImplementedException();
//        }*/

//        private bool ReadNil(bool ignoreLength = false)
//        {
//            if (!ignoreLength)
//            {
//                _stream.Read(_buffer, 0, 1);
//                byte length = _buffer[0];
//                if (length != 3) return false;
//            }

//            _stream.Read(_buffer, 0, 3);
//            if (_buffer[0] == 'n' && _buffer[1] == 'i' && _buffer[2] == 'l')
//                return true;

//            return false;
//        }

//        #region Emit
//        private static Func<ETFReader, T> CreateDeserializer<T>(Type type, TypeInfo typeInfo)
//            where T : new()
//        {
//            var method = new DynamicMethod("DeserializeETF", type, new[] { typeof(ETFReader) }, true);
//            var generator = method.GetILGenerator();

//            generator.Emit(OpCodes.Ldarg_0); //ETFReader(this)
//            EmitReadValue(generator, type, typeInfo, true);

//            generator.Emit(OpCodes.Ret);
//            return method.CreateDelegate(typeof(Func<ETFReader, T>)) as Func<ETFReader, T>;
//        }
//        private static void EmitReadValue(ILGenerator generator, Type type, TypeInfo typeInfo, bool isTop)
//        {
//            //Convert enum types to their base type
//            if (typeInfo.IsEnum)
//            {
//                type = Enum.GetUnderlyingType(type);
//                typeInfo = type.GetTypeInfo();
//            }
//            //Primitives/Enums
//            if (!typeInfo.IsEnum && IsType(type, typeof(sbyte), typeof(byte), typeof(short),
//                            typeof(ushort), typeof(int), typeof(uint), typeof(long),
//                            typeof(ulong), typeof(double), typeof(bool), typeof(string),
//                            typeof(sbyte?), typeof(byte?), typeof(short?), typeof(ushort?),
//                            typeof(int?), typeof(uint?), typeof(long?), typeof(ulong?),
//                            typeof(bool?), typeof(float?), typeof(double?)
//                            /*typeof(object), typeof(DateTime)*/))
//            {
//                //No conversion needed
//                generator.EmitCall(OpCodes.Call, GetReadMethod(type), null);
//            }
//            //Dictionaries
//            /*else if (!typeInfo.IsValueType && typeInfo.ImplementedInterfaces
//                    .Any(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
//            {
//                generator.EmitCall(OpCodes.Call, _writeDictionaryTMethod.MakeGenericMethod(typeInfo.GenericTypeParameters), null);
//            }
//            //Enumerable
//            else if (!typeInfo.IsValueType && typeInfo.ImplementedInterfaces
//                    .Any(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
//            {
//                generator.EmitCall(OpCodes.Call, _writeEnumerableTMethod.MakeGenericMethod(typeInfo.GenericTypeParameters), null);
//            }
//            //Nullable Structs
//            else if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>) &&
//                    typeInfo.GenericTypeParameters[0].GetTypeInfo().IsValueType)
//            {
//                generator.EmitCall(OpCodes.Call, _writeNullableTMethod.MakeGenericMethod(typeInfo.GenericTypeParameters), null);
//            }
//            //Structs/Classes
//            else if (typeInfo.IsClass || (typeInfo.IsValueType && !typeInfo.IsPrimitive))
//            {
//                if (isTop)
//                {
//                    typeInfo.ForEachField(f =>
//                    {
//                        string name;
//                        if (!f.IsPublic || !IsETFProperty(f, out name)) return;

//                        generator.Emit(OpCodes.Ldarg_0); //ETFReader(this)
//                        generator.Emit(OpCodes.Ldstr, name); //ETFReader(this), name
//                        generator.EmitCall(OpCodes.Call, GetWriteMethod(typeof(string)), null);
//                        generator.Emit(OpCodes.Ldarg_0); //ETFReader(this)
//                        generator.Emit(OpCodes.Ldarg_1); //ETFReader(this), obj
//                        generator.Emit(OpCodes.Ldfld, f); //ETFReader(this), obj.fieldValue
//                        EmitWriteValue(generator, f.FieldType, f.FieldType.GetTypeInfo(), false);
//                    });

//                    typeInfo.ForEachProperty(p =>
//                    {
//                        string name;
//                        if (!p.CanRead || !p.GetMethod.IsPublic || !IsETFProperty(p, out name)) return;

//                        generator.Emit(OpCodes.Ldarg_0); //ETFReader(this)
//                        generator.Emit(OpCodes.Ldstr, name); //ETFReader(this), name
//                        generator.EmitCall(OpCodes.Call, GetWriteMethod(typeof(string)), null);
//                        generator.Emit(OpCodes.Ldarg_0); //ETFReader(this)
//                        generator.Emit(OpCodes.Ldarg_1); //ETFReader(this), obj
//                        generator.EmitCall(OpCodes.Callvirt, p.GetMethod, null); //ETFReader(this), obj.propValue
//                        EmitWriteValue(generator, p.PropertyType, p.PropertyType.GetTypeInfo(), false);
//                    });
//                }
//                else
//                {
//                    //While we could drill deeper and make a large serializer that also serializes all subclasses, 
//                    //it's more efficient to serialize on a per-type basis via another Write<T> call.
//                    generator.EmitCall(OpCodes.Call, _writeTMethod.MakeGenericMethod(typeInfo.GenericTypeParameters), null);
//                }
//            }*/
//            //Unsupported (decimal, char)
//            else
//                throw new InvalidOperationException($"Deserializing {type.Name} is not supported.");
//        }

//        private static bool IsType(Type type, params Type[] types)
//        {
//            for (int i = 0; i < types.Length; i++)
//            {
//                if (type == types[i])
//                    return true;
//            }
//            return false;
//        }
//        private static bool IsETFProperty(FieldInfo f, out string name)
//        {
//            var attrib = f.CustomAttributes.Where(x => x.AttributeType == typeof(JsonPropertyAttribute)).FirstOrDefault();
//            if (attrib != null)
//            {
//                name = attrib.ConstructorArguments.FirstOrDefault().Value as string ?? f.Name;
//                return true;
//            }
//            name = null;
//            return false;
//        }
//        private static bool IsETFProperty(PropertyInfo p, out string name)
//        {
//            var attrib = p.CustomAttributes.Where(x => x.AttributeType == typeof(JsonPropertyAttribute)).FirstOrDefault();
//            if (attrib != null)
//            {
//                name = attrib.ConstructorArguments.FirstOrDefault().Value as string ?? p.Name;
//                return true;
//            }
//            name = null;
//            return false;
//        }

//        private static MethodInfo GetReadMethod(string name)
//            => typeof(ETFReader).GetTypeInfo().GetDeclaredMethods(name).Single();
//        private static MethodInfo GetReadMethod(Type type)
//        {
//            MethodInfo method;
//            if (_readMethods.TryGetValue(type, out method))
//                return method;
//            return null;
//        }
//        private static Dictionary<Type, MethodInfo> GetPrimitiveReadMethods()
//        {
//            return new Dictionary<Type, MethodInfo>
//            {
//                { typeof(bool), GetReadMethod(nameof(ReadBool)) },
//                { typeof(bool?), GetReadMethod(nameof(ReadNullableBool)) },
//                { typeof(byte), GetReadMethod(nameof(ReadByte)) },
//                { typeof(byte?), GetReadMethod(nameof(ReadNullableByte)) },
//                { typeof(sbyte), GetReadMethod(nameof(ReadSByte)) },
//                { typeof(sbyte?), GetReadMethod(nameof(ReadNullableSByte)) },
//                { typeof(short), GetReadMethod(nameof(ReadShort)) },
//                { typeof(short?), GetReadMethod(nameof(ReadNullableShort)) },
//                { typeof(ushort), GetReadMethod(nameof(ReadUShort)) },
//                { typeof(ushort?), GetReadMethod(nameof(ReadNullableUShort)) },
//                { typeof(int), GetReadMethod(nameof(ReadInt)) },
//                { typeof(int?), GetReadMethod(nameof(ReadNullableInt)) },
//                { typeof(uint), GetReadMethod(nameof(ReadUInt)) },
//                { typeof(uint?), GetReadMethod(nameof(ReadNullableUInt)) },
//                { typeof(long), GetReadMethod(nameof(ReadLong)) },
//                { typeof(long?), GetReadMethod(nameof(ReadNullableLong)) },
//                { typeof(ulong), GetReadMethod(nameof(ReadULong)) },
//                { typeof(ulong?), GetReadMethod(nameof(ReadNullableULong)) },
//                { typeof(float), GetReadMethod(nameof(ReadSingle)) },
//                { typeof(float?), GetReadMethod(nameof(ReadNullableSingle)) },
//                { typeof(double), GetReadMethod(nameof(ReadDouble)) },
//                { typeof(double?), GetReadMethod(nameof(ReadNullableDouble)) },
//            };
//        }
//        #endregion

//        #region IDisposable
//        private bool _isDisposed = false;

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!_isDisposed)
//            {
//                if (disposing)
//                {
//                    if (_leaveOpen)
//                        _stream.Flush();
//                    else
//                        _stream.Dispose();
//                }
//                _isDisposed = true;
//            }
//        }

//        public void Dispose() => Dispose(true);
//        #endregion
//    }
//}