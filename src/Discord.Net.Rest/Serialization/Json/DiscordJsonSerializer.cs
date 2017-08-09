using Discord.Serialization.Json.Converters;
using System;
using System.Reflection;

namespace Discord.Serialization.Json
{
    internal class DiscordJsonSerializer : JsonSerializer
    {
        private static readonly Lazy<DiscordJsonSerializer> _singleton = new Lazy<DiscordJsonSerializer>();
        public static new DiscordJsonSerializer Global => _singleton.Value;

        public DiscordJsonSerializer()
        {
            AddConverter<API.Image, ImagePropertyConverter>();
            AddConverter<long, Int53PropertyConverter>((type, prop) => prop?.GetCustomAttribute<Int53Attribute>() != null);
            AddConverter<ulong, UInt53PropertyConverter>((type, prop) => prop?.GetCustomAttribute<Int53Attribute>() != null);
            AddGenericConverter(typeof(API.EntityOrId<>), typeof(EntityOrIdPropertyConverter<>));
            AddGenericConverter(typeof(Optional<>), typeof(OptionalPropertyConverter<>));
        }
        protected DiscordJsonSerializer(JsonSerializer parent) : base(parent) { }
        public new DiscordJsonSerializer CreateScope() => new DiscordJsonSerializer(this);
    }
}
