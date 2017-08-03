using Discord.Serialization.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Serialization
{
    internal class Serializer
    {
        public static ScopedSerializer Global { get; } = new ScopedSerializer();

        public static T FromJson<T>(Stream stream) => Global.FromJson<T>(stream);
        public static T FromJson<T>(StreamReader reader) => Global.FromJson<T>(reader);
        public static T FromJson<T>(JsonTextReader reader) => Global.FromJson<T>(reader);
        public static T FromJson<T>(JToken token) => Global.FromJson<T>(token);

        public static void ToJson<T>(Stream stream, T obj) => Global.ToJson(stream, obj);
        public static void ToJson<T>(StreamWriter writer, T obj) => Global.ToJson(writer, obj);
        public static void ToJson<T>(JsonTextWriter writer, T obj) => Global.ToJson(writer, obj);

        public static ScopedSerializer CreateScope() => new ScopedSerializer();
    }

    internal class ScopedSerializer
    {
        private readonly JsonSerializer _serializer;

        private readonly AsyncEvent<Func<Exception, Task>> _errorEvent = new AsyncEvent<Func<Exception, Task>>();
        public event Func<Exception, Task> Error
        {
            add { _errorEvent.Add(value); }
            remove { _errorEvent.Remove(value); }
        }

        internal ScopedSerializer()
        {
            _serializer = new JsonSerializer
            {
                DateFormatString = "yyyy-MM-ddTHH:mm:ssZ",
                ContractResolver = new DiscordContractResolver()
            };
            _serializer.Error += (s, e) =>
            {
                _errorEvent.InvokeAsync(e.ErrorContext.Error).GetAwaiter().GetResult();
                e.ErrorContext.Handled = true;
            };
        }

        public T FromJson<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true)) //1KB buffer
                return FromJson<T>(reader);
        }
        public T FromJson<T>(TextReader reader)
        {
            using (var jsonReader = new JsonTextReader(reader) { CloseInput = false }) 
                return FromJson<T>(jsonReader);
        }
        public T FromJson<T>(JsonTextReader reader)
            => _serializer.Deserialize<T>(reader);
        public T FromJson<T>(JToken token)
            => token.ToObject<T>(_serializer);

        public void ToJson<T>(Stream stream, T obj)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, false)) //1KB buffer
                ToJson(writer, obj);
        }
        public void ToJson<T>(TextWriter writer, T obj)
        {
            using (var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false })
                ToJson(jsonWriter, obj);
        }
        public void ToJson<T>(JsonTextWriter writer, T obj)
            => _serializer.Serialize(writer, obj);
    }
}
