using Discord.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Converters
{
    internal class DiscordErrorConverter : JsonConverter
    {
        public static DiscordErrorConverter Instance
            => new DiscordErrorConverter();

        public override bool CanConvert(Type objectType) => objectType == typeof(DiscordError);

        public override bool CanRead => true;
        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var err = new API.DiscordError();


            var result = obj.GetValue("errors", StringComparison.OrdinalIgnoreCase);
            result?.Parent.Remove();

            // Populate the remaining properties.
            using (var subReader = obj.CreateReader())
            {
                serializer.Populate(subReader, err);
            }

            if (result != null)
            {
                var innerReader = result.CreateReader();

                var errors = ReadErrors(innerReader);
                err.Errors = errors.ToArray();
            }

            return err;
        }

        private List<ErrorDetails> ReadErrors(JsonReader reader, string path = "")
        {
            List<ErrorDetails> errs = new List<ErrorDetails>();
            var obj = JObject.Load(reader);
            var props = obj.Properties();
            foreach (var prop in props)
            {
                if (prop.Name == "_errors" && path == "") // root level error
                {
                    errs.Add(new ErrorDetails()
                    {
                        Name = Optional<string>.Unspecified,
                        Errors = prop.Value.ToObject<Error[]>()
                    });
                }
                else if (prop.Name == "_errors") // path errors (not root level)
                {
                    errs.Add(new ErrorDetails()
                    {
                        Name = path,
                        Errors = prop.Value.ToObject<Error[]>()
                    });
                }
                else if (int.TryParse(prop.Name, out var i)) // array value
                {
                    var r = prop.Value.CreateReader();
                    errs.AddRange(ReadErrors(r, path + $"[{i}]"));
                }
                else // property name
                {
                    var r = prop.Value.CreateReader();
                    errs.AddRange(ReadErrors(r, path + $"{(path != "" ? "." : "")}{prop.Name[0].ToString().ToUpper() + new string(prop.Name.Skip(1).ToArray())}"));
                }
            }

            return errs;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
