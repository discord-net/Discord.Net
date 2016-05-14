using Discord.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Discord.Net.Converters
{
    public class OptionalContractResolver : DefaultContractResolver
    {
        private static readonly PropertyInfo _isSpecified = typeof(IOptional).GetProperty(nameof(IOptional.IsSpecified));

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var type = property.PropertyType;

            if (member.MemberType == MemberTypes.Property)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Optional<>))
                {
                    var parentArg = Expression.Parameter(typeof(object));
                    var optional = Expression.Property(Expression.Convert(parentArg, property.DeclaringType), member as PropertyInfo);
                    var isSpecified = Expression.Property(optional, _isSpecified);
                    var lambda = Expression.Lambda<Func<object, bool>>(isSpecified, parentArg).Compile();
                    property.ShouldSerialize = x => lambda(x);
                }
            }

            return property;
        }
    }
}
