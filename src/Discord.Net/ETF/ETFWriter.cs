using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Discord.ETF
{
    public unsafe class ETFWriter : IDisposable
    {
        private readonly static byte[] _nilBytes = new byte[] { (byte)ETFType.SMALL_ATOM_EXT, 3, (byte)'n', (byte)'i', (byte)'l' };
        private readonly static byte[] _nilExtBytes = new byte[] { (byte)ETFType.NIL_EXT};
        private readonly static byte[] _falseBytes = new byte[] { (byte)ETFType.SMALL_ATOM_EXT, 5, (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e' };
        private readonly static byte[] _trueBytes = new byte[] { (byte)ETFType.SMALL_ATOM_EXT, 4, (byte)'t', (byte)'r', (byte)'u', (byte)'e' };

        private readonly static MethodInfo _writeTMethod = typeof(ETFWriter).GetTypeInfo()
            .GetDeclaredMethods(nameof(Write))
            .Where(x => x.IsGenericMethodDefinition && x.GetParameters()[0].ParameterType == x.GetGenericArguments()[0])
            .Single();
        private readonly static MethodInfo _writeNullableTMethod = typeof(ETFWriter).GetTypeInfo()
            .GetDeclaredMethods(nameof(Write))
            .Where(x =>
            {
                if (!x.IsGenericMethodDefinition) return false;
                var p = x.GetParameters()[0].ParameterType.GetTypeInfo();
                return p.IsGenericType && p.GetGenericTypeDefinition() == typeof(Nullable<>);
            })
            .Single();
        private readonly static DateTime _epochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly Stream _stream;
        private readonly byte[] _buffer;
        private readonly bool _leaveOpen;
        private readonly Encoding _encoding;
        private readonly ConcurrentDictionary<Type, Delegate> _serializers, _indirectSerializers;

        public virtual Stream BaseStream
        {
            get
            {
                Flush();
                return _stream;
            }
        }
        
        public ETFWriter(Stream stream, bool leaveOpen = false)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            _stream = stream;
            _leaveOpen = leaveOpen;
            _buffer = new byte[11];
            _encoding = Encoding.UTF8;
            _serializers = new ConcurrentDictionary<Type, Delegate>();
            _indirectSerializers = new ConcurrentDictionary<Type, Delegate>();
        }

        enum EnumTest1 { A, B, C }
        public static byte[] Test()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new ETFWriter(stream))
                {
                    var request = new API.Client.Rest.SendMessageRequest(109384029348)
                    {
                        Content = "TestMsg",
                        Nonce = null,
                        IsTTS = false
                    };
                    writer.Write(request);
                    /*writer.Write<EnumTest1>((EnumTest1?)EnumTest1.C);
                    writer.Write((object)(EnumTest1?)EnumTest1.C);
                    writer.Write<EnumTest1>((EnumTest1?)null);
                    writer.Write((object)(EnumTest1?)null);*/
                }
                return stream.ToArray();
            }
        }
        
        public void Write(bool value)
        {
            if (value)
                _stream.Write(_trueBytes, 0, _trueBytes.Length);
            else
                _stream.Write(_falseBytes, 0, _falseBytes.Length);
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
                _buffer[1] = (byte)((value >> 24) & 0xFF);
                _buffer[2] = (byte)((value >> 16) & 0xFF);
                _buffer[3] = (byte)((value >> 8) & 0xFF);
                _buffer[4] = (byte)(value & 0xFF);
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
                    _buffer[3 + bytes++] = (byte)((value >>= 8) & 0xFF);
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
                _buffer[1] = (byte)((value >> 24) & 0xFF);
                _buffer[2] = (byte)((value >> 16) & 0xFF);
                _buffer[3] = (byte)((value >> 8) & 0xFF);
                _buffer[4] = (byte)(value & 0xFF);
                _stream.Write(_buffer, 0, 5);
            }
            else
            {
                _buffer[0] = (byte)ETFType.SMALL_BIG_EXT;
                _buffer[2] = 0; //Always positive

                byte bytes = 0;
                while (value > 0)
                    _buffer[3 + bytes++] = (byte)((value >>= 8) & 0xFF);
                _buffer[1] = bytes; //Encoded bytes

                _stream.Write(_buffer, 0, 3 + bytes);
            }
        }

        public void Write(float value) => Write((double)value);
        public void Write(double value)
        {
            ulong value2 = *(ulong*)&value;
            _buffer[0] = (byte)ETFType.NEW_FLOAT_EXT;
            _buffer[1] = (byte)((value2 >> 56) & 0xFF);
            _buffer[2] = (byte)((value2 >> 48) & 0xFF);
            _buffer[3] = (byte)((value2 >> 40) & 0xFF);
            _buffer[4] = (byte)((value2 >> 32) & 0xFF);
            _buffer[5] = (byte)((value2 >> 24) & 0xFF);
            _buffer[6] = (byte)((value2 >> 16) & 0xFF);
            _buffer[7] = (byte)((value2 >> 8) & 0xFF);
            _buffer[8] = (byte)(value2 & 0xFF);
            _stream.Write(_buffer, 0, 9);
        }

        public void Write(DateTime value) => Write((ulong)((value.Ticks - _epochTime.Ticks) / TimeSpan.TicksPerMillisecond));

        public void Write(byte? value) { if (value.HasValue) Write((ulong)value.Value); else WriteNil(); }
        public void Write(sbyte? value) { if (value.HasValue) Write((long)value.Value); else WriteNil(); }
        public void Write(ushort? value) { if (value.HasValue) Write((ulong)value.Value); else WriteNil(); }
        public void Write(short? value) { if (value.HasValue) Write((long)value.Value); else WriteNil(); }
        public void Write(uint? value) { if (value.HasValue) Write((ulong)value.Value); else WriteNil(); }
        public void Write(int? value) { if (value.HasValue) Write(value.Value); else WriteNil(); }
        public void Write(ulong? value) { if (value.HasValue) Write(value.Value); else WriteNil(); }
        public void Write(long? value) { if (value.HasValue) Write(value.Value); else WriteNil(); }
        public void Write(float? value) { if (value.HasValue) Write((double)value.Value); else WriteNil(); }
        public void Write(double? value) { if (value.HasValue) Write(value.Value); else WriteNil(); }
        public void Write(DateTime? value) { if (value.HasValue) Write(value.Value); else WriteNil(); }

        public void Write(byte[] value)
        {
            if (value != null)
            {
                int count = value.Length;
                _buffer[0] = (byte)ETFType.BINARY_EXT;
                _buffer[1] = (byte)((count >> 24) & 0xFF);
                _buffer[2] = (byte)((count >> 16) & 0xFF);
                _buffer[3] = (byte)((count >> 8) & 0xFF);
                _buffer[4] = (byte)(count & 0xFF);
                _stream.Write(_buffer, 0, 5);
                _stream.Write(value, 0, value.Length);
            }
            else
                WriteNil();
        }
        public void Write(string value)
        {
            if (value != null)
            {
                var bytes = _encoding.GetBytes(value);
                int count = bytes.Length;
                _buffer[0] = (byte)ETFType.BINARY_EXT;
                _buffer[1] = (byte)((count >> 24) & 0xFF);
                _buffer[2] = (byte)((count >> 16) & 0xFF);
                _buffer[3] = (byte)((count >> 8) & 0xFF);
                _buffer[4] = (byte)(count & 0xFF);
                _stream.Write(_buffer, 0, 5);
                _stream.Write(bytes, 0, bytes.Length);
            }
            else
                WriteNil();
        }

        public void Write<T>(T obj)
        {
            var type = typeof(T);
            var typeInfo = type.GetTypeInfo();
            var action = _serializers.GetOrAdd(type, _ => CreateSerializer<T>(type, typeInfo, false)) as Action<ETFWriter, T>;
            action(this, obj);
        }
        public void Write<T>(T? obj)
            where T : struct
        {
            if (obj != null)
                Write(obj.Value);
            else
                WriteNil();
        }
        public void Write<T>(IEnumerable<T> obj)
        {
            if (obj != null)
            {
                var array = obj.ToArray();
                int length = array.Length;
                _buffer[0] = (byte)ETFType.LIST_EXT;
                _buffer[1] = (byte)((length >> 24) & 0xFF);
                _buffer[2] = (byte)((length >> 16) & 0xFF);
                _buffer[3] = (byte)((length >> 8) & 0xFF);
                _buffer[4] = (byte)(length & 0xFF);
                for (int i = 0; i < array.Length; i++)
                    Write(array[i]);
                WriteNilExt();
                _stream.Write(_buffer, 0, 5);
            }
            else
                WriteNil();
        }
        public void Write<TKey, TValue>(IDictionary<TKey, TValue> obj)
        {
            if (obj != null)
            {
                int length = obj.Count;
                _buffer[0] = (byte)ETFType.MAP_EXT;
                _buffer[1] = (byte)((length >> 24) & 0xFF);
                _buffer[2] = (byte)((length >> 16) & 0xFF);
                _buffer[3] = (byte)((length >> 8) & 0xFF);
                _buffer[4] = (byte)(length & 0xFF);
                foreach (var pair in obj)
                {
                    Write(pair.Key);
                    Write(pair.Value);
                }
                _stream.Write(_buffer, 0, 5);
            }
            else
                WriteNil();
        }
        public void Write(object obj)
        {
            if (obj != null)
            {
                var type = obj.GetType();
                var typeInfo = type.GetTypeInfo();
                var action = _indirectSerializers.GetOrAdd(type, _ => CreateSerializer<object>(type, typeInfo, true)) as Action<ETFWriter, object>;
                action(this, obj);
            }
            else
                WriteNil();
        }

        public virtual void Flush() => _stream.Flush();
        public virtual long Seek(int offset, SeekOrigin origin) => _stream.Seek(offset, origin);
        
        private Action<ETFWriter, T> CreateSerializer<T>(Type type, TypeInfo typeInfo, bool isObject)
        {
            var method = new DynamicMethod(!isObject ? "SerializeETF" : "SerializeIndirectETF", 
                null, new[] { typeof(ETFWriter), isObject? typeof(object) : type }, true);
            var generator = method.GetILGenerator();

            if (typeInfo.IsPrimitive || typeInfo.IsEnum)
            {
                generator.Emit(OpCodes.Ldarg_0); //ETFWriter(this)
                generator.Emit(OpCodes.Ldarg_1); //ETFWriter(this), value
                if (isObject && typeInfo.IsValueType)
                    generator.Emit(OpCodes.Unbox_Any, type); //ETFWriter(this), value
                EmitETFWriteValue(generator, type, typeInfo);
            }
            else
            {
                //Scan for certain interfaces
                Type dictionaryI = null, enumerableI = null;
                foreach (var i in typeInfo.ImplementedInterfaces
                    .Where(x => x.IsConstructedGenericType))
                {
                    if (i.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    {
                        //TODO: Emit null check
                        dictionaryI = i;
                        break;
                    }
                    else if (i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        //TODO: Emit null check
                        enumerableI = i;
                        break;
                    }
                }

                if (dictionaryI != null)
                {
                    throw new NotImplementedException();
                }
                else if (enumerableI != null)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    //TODO: Add field/property names
                    typeInfo.ForEachField(f =>
                    {
                        string name;
                        if (!f.IsPublic || !IsETFProperty(f, out name)) return;

                        generator.Emit(OpCodes.Ldarg_0); //ETFWriter(this)
                        generator.Emit(OpCodes.Ldstr, name); //ETFWriter(this), name
                        generator.EmitCall(OpCodes.Call, GetWriteMethod(typeof(string)), null);
                        generator.Emit(OpCodes.Ldarg_0); //ETFWriter(this)
                        generator.Emit(OpCodes.Ldarg_1); //ETFWriter(this), obj
                        generator.Emit(OpCodes.Ldfld, f); //ETFWriter(this), obj.fieldValue
                        if (isObject && typeInfo.IsValueType)
                            generator.Emit(OpCodes.Unbox_Any, type); //ETFWriter(this), obj.fieldValue
                        EmitETFWriteValue(generator, f.FieldType);
                    });

                    typeInfo.ForEachProperty(p =>
                    {
                        string name;
                        if (!p.CanRead || !p.GetMethod.IsPublic || !IsETFProperty(p, out name)) return;

                        generator.Emit(OpCodes.Ldarg_0); //ETFWriter(this)
                        generator.Emit(OpCodes.Ldstr, name); //ETFWriter(this), name
                        generator.EmitCall(OpCodes.Call, GetWriteMethod(typeof(string)), null);
                        generator.Emit(OpCodes.Ldarg_0); //ETFWriter(this)
                        generator.Emit(OpCodes.Ldarg_1); //ETFWriter(this), obj
                        generator.EmitCall(OpCodes.Callvirt, p.GetMethod, null); //ETFWriter(this), obj.propValue
                        if (isObject && typeInfo.IsValueType)
                            generator.Emit(OpCodes.Unbox_Any, type); //ETFWriter(this), obj.propValue
                        EmitETFWriteValue(generator, p.PropertyType);
                    });
                }
            }

            generator.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Action<ETFWriter, T>)) as Action<ETFWriter, T>;
        }

        private void EmitETFWriteValue(ILGenerator generator, Type type, TypeInfo typeInfo = null)
        {
            if (typeInfo == null)
                typeInfo = type.GetTypeInfo();
            
            //Convert enum types to their base type
            if (typeInfo.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
                typeInfo = type.GetTypeInfo();
            }            
            
            //Check if this type already has a direct call or simple conversion
            Type targetType = null;
            if (!typeInfo.IsEnum && IsType(type, typeof(long), typeof(ulong), typeof(double), typeof(bool), typeof(string),
                             typeof(sbyte?), typeof(byte?), typeof(short?), typeof(ushort?),
                             typeof(int?), typeof(uint?), typeof(long?), typeof(ulong?),
                             typeof(bool?), typeof(float?), typeof(double?),
                             typeof(object), typeof(DateTime)))
                targetType = type; //Direct
            else if (IsType(type, typeof(sbyte), typeof(short), typeof(int)))
            {
                generator.Emit(OpCodes.Conv_I8);
                targetType = typeof(long);
            }
            else if (IsType(type, typeof(byte), typeof(ushort), typeof(uint)))
            {
                generator.Emit(OpCodes.Conv_U8);
                targetType = typeof(ulong);
            }
            else if (IsType(type, typeof(float)))
            {
                generator.Emit(OpCodes.Conv_R8);
                targetType = typeof(double);
            }

            //Primitives/Enums
            if (targetType != null)
                generator.EmitCall(OpCodes.Call, GetWriteMethod(targetType), null);

            //Nullable Non-Primitives
            else if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
                generator.EmitCall(OpCodes.Call, _writeNullableTMethod.MakeGenericMethod(typeInfo.GenericTypeParameters), null);

            //Structs/Classes
            else if (typeInfo.IsClass || (typeInfo.IsValueType && !typeInfo.IsPrimitive))
                generator.EmitCall(OpCodes.Call, _writeTMethod.MakeGenericMethod(type), null);

            //Unsupported (decimal, char)
            else
                throw new InvalidOperationException($"Serializing {type.Name} is not supported.");
        }

        private bool IsType(Type type, params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (type == types[i])
                    return true;
            }
            return false;
        }
        private MethodInfo GetWriteMethod(Type paramType)
        {
            return typeof(ETFWriter).GetTypeInfo().GetDeclaredMethods(nameof(Write))
                .Where(x => x.GetParameters()[0].ParameterType == paramType)
                .FirstOrDefault();
        }

        private void WriteNil() => _stream.Write(_nilBytes, 0, _nilBytes.Length);
        private void WriteNilExt() => _stream.Write(_nilExtBytes, 0, _nilExtBytes.Length);

        private bool IsETFProperty(FieldInfo f, out string name)
        {
            var attrib = f.CustomAttributes.Where(x => x.AttributeType == typeof(JsonPropertyAttribute)).FirstOrDefault();
            if (attrib != null)
            {
                name = attrib.ConstructorArguments.FirstOrDefault().Value as string ?? f.Name;
                return true;
            }
            name = null;
            return false;
        }
        private bool IsETFProperty(PropertyInfo p, out string name)
        {
            var attrib = p.CustomAttributes.Where(x => x.AttributeType == typeof(JsonPropertyAttribute)).FirstOrDefault();
            if (attrib != null)
            {
                name = attrib.ConstructorArguments.FirstOrDefault().Value as string ?? p.Name;
                return true;
            }
            name = null;
            return false;
        }

        #region IDisposable
        private bool _isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (_leaveOpen)
                        _stream.Flush();
                    else
                        _stream.Dispose();
                }
                _isDisposed = true;
            }
        }
        
        public void Dispose() => Dispose(true);
        #endregion
    }
}
