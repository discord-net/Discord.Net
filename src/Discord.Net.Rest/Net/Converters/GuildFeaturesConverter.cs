using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Discord.Net.Converters
{
    internal class GuildFeaturesConverter : JsonConverter
    {
        public static GuildFeaturesConverter Instance
            => new GuildFeaturesConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JToken.Load(reader);
            var arr = obj.ToObject<string[]>();

            GuildFeature features = GuildFeature.None;
            List<string> experimental = new();

            foreach (var item in arr)
            {
                if (Enum.TryParse<GuildFeature>(string.Concat(item.Split('_')), true, out var result))
                {
                    features |= result;
                }
                else
                {
                    experimental.Add(item);
                }
            }

            return new GuildFeatures(features, experimental.ToArray());
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
