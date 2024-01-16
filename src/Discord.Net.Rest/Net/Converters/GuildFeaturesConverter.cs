using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

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
            var guildFeatures = (GuildFeatures)value;

            var enumValues = Enum.GetValues(typeof(GuildFeature));

            writer.WriteStartArray();

            foreach (var enumValue in enumValues)
            {
                var val = (GuildFeature)enumValue;
                if (val is GuildFeature.None)
                    continue;

                if (guildFeatures.Value.HasFlag(val))
                {
                    writer.WriteValue(FeatureToApiString(val));
                }
            }
            writer.WriteEndArray();
        }

        private string FeatureToApiString(GuildFeature feature)
        {
            var builder = new StringBuilder();
            var firstChar = true;

            foreach (var c in feature.ToString().ToCharArray())
            {
                if (char.IsUpper(c))
                {
                    if (firstChar)
                        firstChar = false;
                    else
                        builder.Append("_");

                    builder.Append(c);
                }
                else
                {
                    builder.Append(char.ToUpper(c));
                }
            }

            return builder.ToString();
        }
    }
}
