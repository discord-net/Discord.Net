using Discord.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Discord.Net.Converters
{
    public class DiscordContractResolver : DefaultContractResolver
    {        
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var type = property.PropertyType;
            JsonConverter converter = null;

            if (member.MemberType == MemberTypes.Property)
            {
                //Primitives
                if (type == typeof(ulong) && member.GetCustomAttribute<Int53Attribute>() == null)
                    converter = UInt64Converter.Instance;
                else if (type == typeof(ulong?) && member.GetCustomAttribute<Int53Attribute>() == null)
                    converter = NullableUInt64Converter.Instance;
                else if (typeof(IEnumerable<ulong[]>).IsAssignableFrom(type) && member.GetCustomAttribute<Int53Attribute>() == null)
                    converter = NullableUInt64Converter.Instance;

                //Enums
                else if (type == typeof(ChannelType))
                    converter = ChannelTypeConverter.Instance;
                else if (type == typeof(PermissionTarget))
                    converter = PermissionTargetConverter.Instance;
                else if (type == typeof(UserStatus))
                    converter = UserStatusConverter.Instance;

                //Entities
                else if (typeof(IEntity<ulong>).IsAssignableFrom(type))
                    converter = UInt64EntityConverter.Instance;
                else if (typeof(IEntity<string>).IsAssignableFrom(type))
                    converter = StringEntityConverter.Instance;

                //Special
                else if (type == typeof(string) && member.GetCustomAttribute<ImageAttribute>() != null)
                    converter = ImageConverter.Instance;
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Optional<>))
                {
                    var parentArg = Expression.Parameter(typeof(object));
                    var optional = Expression.Property(Expression.Convert(parentArg, property.DeclaringType), member as PropertyInfo);
                    var isSpecified = Expression.Property(optional, OptionalConverter.IsSpecifiedProperty);
                    var lambda = Expression.Lambda<Func<object, bool>>(isSpecified, parentArg).Compile();
                    property.ShouldSerialize = x => lambda(x);
                    converter = OptionalConverter.Instance;
                }
            }
            if (converter != null)
            {
                property.Converter = converter;
                property.MemberConverter = converter;
            }

            return property;
        }
    }
}
