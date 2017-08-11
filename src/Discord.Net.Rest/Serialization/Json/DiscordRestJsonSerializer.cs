using Discord.Serialization.Json.Converters;
using System;
using System.Reflection;

namespace Discord.Serialization.Json
{
    public class DiscordRestJsonSerializer : JsonSerializer
    {
        private static readonly Lazy<DiscordRestJsonSerializer> _singleton = new Lazy<DiscordRestJsonSerializer>();
        public static DiscordRestJsonSerializer Global => _singleton.Value;

        public DiscordRestJsonSerializer()
            : this((JsonSerializer)null) { }
        public DiscordRestJsonSerializer(JsonSerializer parent)
            : base(parent ?? DefaultJsonSerializer.Global)
        {
            AddConverter<API.Image, ImagePropertyConverter>();
            AddConverter<long, Int53PropertyConverter>((type, prop) => prop?.GetCustomAttribute<Int53Attribute>() != null);
            AddConverter<ulong, UInt53PropertyConverter>((type, prop) => prop?.GetCustomAttribute<Int53Attribute>() != null);

            AddGenericConverter(typeof(API.EntityOrId<>), typeof(EntityOrIdPropertyConverter<>));
            AddGenericConverter(typeof(Optional<>), typeof(OptionalPropertyConverter<>));
        }

        private DiscordRestJsonSerializer(DiscordRestJsonSerializer parent)
            : base(parent) { }
        public DiscordRestJsonSerializer CreateScope() => new DiscordRestJsonSerializer(this);
    }
}
