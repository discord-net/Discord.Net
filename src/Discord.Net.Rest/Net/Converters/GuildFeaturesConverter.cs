using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord.Net.Converters
{
    internal class GuildFeaturesConverter : JsonConverter
    {
        public static GuildFeaturesConverter Instance
            => new GuildFeaturesConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanWrite => false;
        public override bool CanRead => true;


        private Regex _readRegex = new Regex(@"_(\w)");

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JToken.Load(reader);
            var arr = obj.ToObject<string[]>();

            GuildFeature features = GuildFeature.None;
            List<string> experimental = new();

            foreach(var item in arr)
            {
                var name = _readRegex.Replace(item.ToLower(), (x) =>
                {
                    return x.Groups[1].Value.ToUpper();
                });

                name = name[0].ToString().ToUpper() + new string(name.Skip(1).ToArray());

                try
                {
                    var result = (GuildFeature)Enum.Parse(typeof(GuildFeature), name);

                    features |= result;
                }
                catch
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
