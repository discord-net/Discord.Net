using Discord.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Discord.Net.Converters
{
    internal class DiscordContractResolver : DefaultContractResolver
    {
        private static readonly TypeInfo _ienumerable = typeof(IEnumerable<ulong[]>).GetTypeInfo();
        private static readonly MethodInfo _shouldSerialize = typeof(DiscordContractResolver).GetTypeInfo().GetDeclaredMethod("ShouldSerialize");    
        
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.Ignored)
                return property;

            var propInfo = member as PropertyInfo;
            if (propInfo != null)
            {
                JsonConverter converter;
                Type type = propInfo.PropertyType;
                Type genericType = type.IsConstructedGenericType ? type.GetGenericTypeDefinition() : null;

                if (genericType == typeof(Optional<>))
                {
                    if (type == typeof(Optional<API.Image?>))
                    {
                        converter = ImageConverter.Instance;
                    }
                    else
                    {
                        var typeInput = propInfo.DeclaringType;
                        var innerTypeOutput = type.GenericTypeArguments[0];

                        var getter = typeof(Func<,>).MakeGenericType(typeInput, type);
                        var getterDelegate = propInfo.GetMethod.CreateDelegate(getter);
                        var shouldSerialize = _shouldSerialize.MakeGenericMethod(typeInput, innerTypeOutput);
                        var shouldSerializeDelegate =
                            (Func<object, Delegate, bool>)
                            shouldSerialize.CreateDelegate(typeof(Func<object, Delegate, bool>));
                        property.ShouldSerialize = x => shouldSerializeDelegate(x, getterDelegate);

                        converter = MakeGenericConverter(propInfo, typeof(OptionalConverter<>), innerTypeOutput);
                    }
                }
                else
                    converter = GetConverter(propInfo, type);

                if (converter != null)
                {
                    property.Converter = converter;
                    property.MemberConverter = converter;
                }
            }
            else
                throw new InvalidOperationException($"{member.DeclaringType.FullName}.{member.Name} is not a property.");
            return property;
        }

        private static JsonConverter GetConverter(PropertyInfo propInfo, Type type, TypeInfo typeInfo = null, int depth = 0)
        {
            if (type.IsArray)
                return MakeGenericConverter(propInfo, typeof(ArrayConverter<>), type.GetElementType());
            if (type.IsConstructedGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(EntityOrId<>))
                    return MakeGenericConverter(propInfo, typeof(UInt64EntityOrIdConverter<>), type.GenericTypeArguments[0]);
            }

            bool hasInt53 = propInfo.GetCustomAttribute<Int53Attribute>() != null;

            //Primitives
            if (!hasInt53)
            {
                if (type == typeof(ulong))
                    return UInt64Converter.Instance;
                if (type == typeof(ulong?))
                    return NullableUInt64Converter.Instance;
            }

            //Enums
            if (type == typeof(PermissionTarget))
                return PermissionTargetConverter.Instance;
            if (type == typeof(UserStatus))
                return UserStatusConverter.Instance;

            //Special
            if (type == typeof(Image))
                return ImageConverter.Instance;            

            if (typeInfo == null) typeInfo = type.GetTypeInfo();

            //Entities
            if (typeInfo.ImplementedInterfaces.Any(x => x == typeof(IEntity<ulong>)))
                return UInt64EntityConverter.Instance;
            if (typeInfo.ImplementedInterfaces.Any(x => x == typeof(IEntity<string>)))
                return StringEntityConverter.Instance;

            return null;
        }

        private static bool ShouldSerialize<TOwner, TValue>(object owner, Delegate getter)
        {
            return (getter as Func<TOwner, Optional<TValue>>)((TOwner)owner).IsSpecified;
        }

        private static JsonConverter MakeGenericConverter(PropertyInfo propInfo, Type converterType, Type innerType)
        {
            var genericType = converterType.MakeGenericType(innerType).GetTypeInfo();
            //var instanceField = genericType.GetDeclaredField("Instance");
            //var converter = instanceField.GetValue(null) as JsonConverter;
            //if (converter == null)
            //{
                var innerConverter = GetConverter(propInfo, innerType);
                var converter = genericType.DeclaredConstructors.First().Invoke(new object[] { innerConverter }) as JsonConverter;
                //instanceField.SetValue(null, converter);
            //}
            return converter;
        }
    }
}
