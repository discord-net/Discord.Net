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
        private static readonly ConcurrentDictionary<Type, Delegate> _serializers = new ConcurrentDictionary<Type, Delegate>();
        private static readonly ConcurrentDictionary<Type, Delegate> _indirectSerializers = new ConcurrentDictionary<Type, Delegate>();

        private static readonly byte[] _nilBytes = new byte[] { (byte)ETFType.SMALL_ATOM_EXT, 3, (byte)'n', (byte)'i', (byte)'l' };
        private static readonly byte[] _falseBytes = new byte[] { (byte)ETFType.SMALL_ATOM_EXT, 5, (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e' };
        private static readonly byte[] _trueBytes = new byte[] { (byte)ETFType.SMALL_ATOM_EXT, 4, (byte)'t', (byte)'r', (byte)'u', (byte)'e' };

        private static readonly MethodInfo _writeTMethod = GetGenericWriteMethod(null);
        private static readonly MethodInfo _writeNullableTMethod = GetGenericWriteMethod(typeof(Nullable<>));
        private static readonly MethodInfo _writeDictionaryTMethod = GetGenericWriteMethod(typeof(IDictionary<,>));
        private static readonly MethodInfo _writeEnumerableTMethod = GetGenericWriteMethod(typeof(IEnumerable<>));
        private static readonly DateTime _epochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly Stream _stream;
        private readonly byte[] _buffer;
        private readonly bool _leaveOpen;
        private readonly Encoding _encoding;

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
                //TODO: Does this encode negatives correctly?
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
        public void Write(double value)
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

        public void Write(bool? value) { if (value.HasValue) Write(value.Value); else WriteNil(); }
        public void Write(sbyte? value) { if (value.HasValue) Write((long)value.Value); else WriteNil(); }
        public void Write(byte? value) { if (value.HasValue) Write((ulong)value.Value); else WriteNil(); }
        public void Write(short? value) { if (value.HasValue) Write((long)value.Value); else WriteNil(); }
        public void Write(ushort? value) { if (value.HasValue) Write((ulong)value.Value); else WriteNil(); }
        public void Write(int? value) { if (value.HasValue) Write(value.Value); else WriteNil(); }
        public void Write(uint? value) { if (value.HasValue) Write((ulong)value.Value); else WriteNil(); }
        public void Write(long? value) { if (value.HasValue) Write(value.Value); else WriteNil(); }
        public void Write(ulong? value) { if (value.HasValue) Write(value.Value); else WriteNil(); }
        public void Write(double? value) { if (value.HasValue) Write(value.Value); else WriteNil(); }
        public void Write(float? value) { if (value.HasValue) Write((double)value.Value); else WriteNil(); }
        public void Write(DateTime? value) { if (value.HasValue) Write(value.Value); else WriteNil(); }

        public void Write(string value)
        {
            if (value != null)
            {
                var bytes = _encoding.GetBytes(value);
                int count = bytes.Length;
                _buffer[0] = (byte)ETFType.BINARY_EXT;
                _buffer[1] = (byte)(count >> 24);
                _buffer[2] = (byte)(count >> 16);
                _buffer[3] = (byte)(count >> 8);
                _buffer[4] = (byte)count;
                _stream.Write(_buffer, 0, 5);
                _stream.Write(bytes, 0, bytes.Length);
            }
            else
                WriteNil();
        }
        public void Write(byte[] value)
        {
            if (value != null)
            {
                int count = value.Length;
                _buffer[0] = (byte)ETFType.BINARY_EXT;
                _buffer[1] = (byte)(count >> 24);
                _buffer[2] = (byte)(count >> 16);
                _buffer[3] = (byte)(count >> 8);
                _buffer[4] = (byte)count;
                _stream.Write(_buffer, 0, 5);
                _stream.Write(value, 0, value.Length);
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
                _buffer[1] = (byte)(length >> 24);
                _buffer[2] = (byte)(length >> 16);
                _buffer[3] = (byte)(length >> 8);
                _buffer[4] = (byte)length;
                _stream.Write(_buffer, 0, 5);

                for (int i = 0; i < array.Length; i++)
                    Write(array[i]);

                _buffer[0] = (byte)ETFType.NIL_EXT;
                _stream.Write(_buffer, 0, 1);
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
                _buffer[1] = (byte)(length >> 24);
                _buffer[2] = (byte)(length >> 16);
                _buffer[3] = (byte)(length >> 8);
                _buffer[4] = (byte)length;
                _stream.Write(_buffer, 0, 5);

                foreach (var pair in obj)
                {
                    Write(pair.Key);
                    Write(pair.Value);
                }
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

        private void WriteNil() => _stream.Write(_nilBytes, 0, _nilBytes.Length);

        public virtual void Flush() => _stream.Flush();
        public virtual long Seek(int offset, SeekOrigin origin) => _stream.Seek(offset, origin);

        #region Emit
        private static Action<ETFWriter, T> CreateSerializer<T>(Type type, TypeInfo typeInfo, bool isDirect)
        {
            var method = new DynamicMethod(isDirect ? "SerializeETF" : "SerializeIndirectETF", 
                null, new[] { typeof(ETFWriter), isDirect ? type : typeof(object) }, true);
            var generator = method.GetILGenerator();
            
            generator.Emit(OpCodes.Ldarg_0); //ETFWriter(this)
            generator.Emit(OpCodes.Ldarg_1); //ETFWriter(this), value
            if (!isDirect)
            {
                if (typeInfo.IsValueType) //Unbox value types
                    generator.Emit(OpCodes.Unbox_Any, type); //ETFWriter(this), real_value
                else //Cast reference types
                    generator.Emit(OpCodes.Castclass, type); //ETFWriter(this), real_value
                generator.EmitCall(OpCodes.Call, _writeTMethod.MakeGenericMethod(type), null); //Call generic version
            }
            else
                EmitWriteValue(generator, type, typeInfo, true);

            generator.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Action<ETFWriter, T>)) as Action<ETFWriter, T>;
        }
        private static void EmitWriteValue(ILGenerator generator, Type type, TypeInfo typeInfo, bool isTop)
        {            
            //Convert enum types to their base type
            if (typeInfo.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
                typeInfo = type.GetTypeInfo();
            }
            
            //Primitives/Enums
            Type targetType = null;
            if (!typeInfo.IsEnum && IsType(type, typeof(long), typeof(ulong), typeof(double), typeof(bool), typeof(string),
                             typeof(sbyte?), typeof(byte?), typeof(short?), typeof(ushort?),
                             typeof(int?), typeof(uint?), typeof(long?), typeof(ulong?),
                             typeof(bool?), typeof(float?), typeof(double?),
                             typeof(object), typeof(DateTime)))
            {
                //No conversion needed
                targetType = type;
            }
            else if (IsType(type, typeof(sbyte), typeof(short), typeof(int)))
            {
                //Convert to long
                generator.Emit(OpCodes.Conv_I8);
                targetType = typeof(long);
            }
            else if (IsType(type, typeof(byte), typeof(ushort), typeof(uint)))
            {
                //Convert to ulong
                generator.Emit(OpCodes.Conv_U8);
                targetType = typeof(ulong);
            }
            else if (IsType(type, typeof(float)))
            {
                //Convert to double
                generator.Emit(OpCodes.Conv_R8);
                targetType = typeof(double);
            }
            if (targetType != null)
                generator.EmitCall(OpCodes.Call, GetWriteMethod(targetType), null);

            //Dictionaries
            else if (!typeInfo.IsValueType && typeInfo.ImplementedInterfaces
                    .Any(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
            {
                generator.EmitCall(OpCodes.Call, _writeDictionaryTMethod.MakeGenericMethod(typeInfo.GenericTypeParameters), null);
            }
            //Enumerable
            else if (!typeInfo.IsValueType && typeInfo.ImplementedInterfaces
                    .Any(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                generator.EmitCall(OpCodes.Call, _writeEnumerableTMethod.MakeGenericMethod(typeInfo.GenericTypeParameters), null);
            }
            //Nullable Structs
            else if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                    typeInfo.GenericTypeParameters[0].GetTypeInfo().IsValueType)
            {
                generator.EmitCall(OpCodes.Call, _writeNullableTMethod.MakeGenericMethod(typeInfo.GenericTypeParameters), null);
            }
            //Structs/Classes
            else if (typeInfo.IsClass || (typeInfo.IsValueType && !typeInfo.IsPrimitive))
            {
                if (isTop)
                {
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
                        EmitWriteValue(generator, f.FieldType, f.FieldType.GetTypeInfo(), false);
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
                        EmitWriteValue(generator, p.PropertyType, p.PropertyType.GetTypeInfo(), false);
                    });
                }
                else
                {
                    //While we could drill deeper and make a large serializer that also serializes all subclasses, 
                    //it's more efficient to serialize on a per-type basis via another Write<T> call.
                    generator.EmitCall(OpCodes.Call, _writeTMethod.MakeGenericMethod(typeInfo.GenericTypeParameters), null);
                }
            }
            //Unsupported (decimal, char)
            else
                throw new InvalidOperationException($"Serializing {type.Name} is not supported.");
        }

        private static bool IsType(Type type, params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (type == types[i])
                    return true;
            }
            return false;
        }
        private static bool IsETFProperty(FieldInfo f, out string name)
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
        private static bool IsETFProperty(PropertyInfo p, out string name)
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

        private static MethodInfo GetWriteMethod(Type paramType)
        {
            return typeof(ETFWriter).GetTypeInfo().GetDeclaredMethods(nameof(Write))
                .Where(x => x.GetParameters()[0].ParameterType == paramType)
                .Single();
        }
        private static MethodInfo GetGenericWriteMethod(Type genericType)
        {
            if (genericType == null)
            {
                return typeof(ETFWriter).GetTypeInfo()
                    .GetDeclaredMethods(nameof(Write))
                    .Where(x => x.IsGenericMethodDefinition && x.GetParameters()[0].ParameterType == x.GetGenericArguments()[0])
                    .Single();
            }
            else
            {
                return typeof(ETFWriter).GetTypeInfo()
                   .GetDeclaredMethods(nameof(Write))
                   .Where(x =>
                   {
                       if (!x.IsGenericMethodDefinition) return false;
                       var p = x.GetParameters()[0].ParameterType.GetTypeInfo();
                       return p.IsGenericType && p.GetGenericTypeDefinition() == genericType;
                   })
                   .Single();
            }
        }
        #endregion

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
