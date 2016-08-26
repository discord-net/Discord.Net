using Discord.Net.Converters;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Discord.Tests.Framework
{
    public class Json
    {
        public static JsonSerializer Serializer;

        public Json()
        {
            Serializer = new JsonSerializer();
            var _serializer = new JsonSerializer();
            _serializer.Converters.Add(new ChannelTypeConverter());
            _serializer.Converters.Add(new ImageConverter());
            _serializer.Converters.Add(new NullableUInt64Converter());
            _serializer.Converters.Add(new PermissionTargetConverter());
            _serializer.Converters.Add(new StringEntityConverter());
            _serializer.Converters.Add(new UInt64Converter());
            _serializer.Converters.Add(new UInt64EntityConverter());
            _serializer.Converters.Add(new UserStatusConverter());
            Serializer = _serializer;
        }

        public static string SerializeObject(object o)
        {
            var sb = new StringBuilder(256);
            using (TextWriter text = new StringWriter(sb, CultureInfo.InvariantCulture))
            using (JsonWriter writer = new JsonTextWriter(text))
                Serializer.Serialize(writer, o);
            return sb.ToString();
        }
    }
}
