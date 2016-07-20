using Discord.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Discord.Net.Converters
{
    public class DiscordContractResolver : DefaultContractResolver
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
                var type = propInfo.PropertyType;

                if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Optional<>))
                {
                    var typeInput = propInfo.DeclaringType;
                    var innerTypeOutput = type.GenericTypeArguments[0];

                    var getter = typeof(Func<,>).MakeGenericType(typeInput, type);
                    var getterDelegate = propInfo.GetMethod.CreateDelegate(getter);
                    var shouldSerialize = _shouldSerialize.MakeGenericMethod(typeInput, innerTypeOutput);
                    var shouldSerializeDelegate = (Func<object, Delegate, bool>)shouldSerialize.CreateDelegate(typeof(Func<object, Delegate, bool>));
                    property.ShouldSerialize = x => shouldSerializeDelegate(x, getterDelegate);

                    var converterType = typeof(OptionalConverter<>).MakeGenericType(innerTypeOutput).GetTypeInfo();
                    var instanceField = converterType.GetDeclaredField("Instance");
                    converter = instanceField.GetValue(null) as JsonConverter;
                    if (converter == null)
                    {
                        var innerConverter = GetConverter(propInfo, innerTypeOutput);
                        converter = converterType.DeclaredConstructors.First().Invoke(new object[] { innerConverter }) as JsonConverter;
                        instanceField.SetValue(null, converter);
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

        private JsonConverter GetConverter(MemberInfo member, Type type, TypeInfo typeInfo = null)
        {
            bool hasInt53 = member.GetCustomAttribute<Int53Attribute>() != null;

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

            //Primitives
            if (!hasInt53 && typeInfo.ImplementedInterfaces.Any(x => x == typeof(IEnumerable<ulong>)))
                return UInt64ArrayConverter.Instance;

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
    }
}
