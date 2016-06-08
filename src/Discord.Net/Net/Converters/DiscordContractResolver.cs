using Discord.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
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
            var propInfo = member as PropertyInfo;

            if (propInfo != null)
            {
                JsonConverter converter = null;
                var type = property.PropertyType;
                var typeInfo = type.GetTypeInfo();

                //Primitives
                if (propInfo.GetCustomAttribute<Int53Attribute>() == null)
                {
                    if (type == typeof(ulong))
                        converter = UInt64Converter.Instance;
                    else if (type == typeof(ulong?))
                        converter = NullableUInt64Converter.Instance;
                    else if (typeInfo.ImplementedInterfaces.Any(x => x == typeof(IEnumerable<ulong>)))
                        converter = UInt64ArrayConverter.Instance;
                }
                if (converter == null)
                { 
                    //Enums
                    if (type == typeof(ChannelType))
                        converter = ChannelTypeConverter.Instance;
                    else if (type == typeof(PermissionTarget))
                        converter = PermissionTargetConverter.Instance;
                    else if (type == typeof(UserStatus))
                        converter = UserStatusConverter.Instance;

                    //Entities
                    if (typeInfo.ImplementedInterfaces.Any(x => x == typeof(IEntity<ulong>)))
                        converter = UInt64EntityConverter.Instance;
                    else if (typeInfo.ImplementedInterfaces.Any(x => x == typeof(IEntity<string>)))
                        converter = StringEntityConverter.Instance;

                    //Special
                    else if (type == typeof(string) && propInfo.GetCustomAttribute<ImageAttribute>() != null)
                        converter = ImageConverter.Instance;
                    else if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Optional<>))
                    {
                        var typeInput = propInfo.DeclaringType;
                        var typeOutput = propInfo.PropertyType;

                        var getter = typeof(Func<,>).MakeGenericType(typeInput, typeOutput);
                        var getterDelegate = propInfo.GetMethod.CreateDelegate(getter);
                        var shouldSerialize = _shouldSerialize.MakeGenericMethod(typeInput, typeOutput);
                        var shouldSerializeDelegate = (Func<object, Delegate, bool>)shouldSerialize.CreateDelegate(typeof(Func<object, Delegate, bool>));
                        
                        property.ShouldSerialize = x => shouldSerializeDelegate(x, getterDelegate);
                        converter = OptionalConverter.Instance;
                    }
                }

                if (converter != null)
                {
                    property.Converter = converter;
                    property.MemberConverter = converter;
                }
            }

            return property;
        }

        private static bool ShouldSerialize<TOwner, TValue>(object owner, Delegate getter)
            where TValue : IOptional
        {
            return (getter as Func<TOwner, TValue>)((TOwner)owner).IsSpecified;
        }
    }
}
