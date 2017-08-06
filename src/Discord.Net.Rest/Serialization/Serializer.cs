using System;
using System.Text;
using System.Text.Formatting;
using System.Text.Json;
using System.Threading.Tasks;

namespace Discord.Serialization
{
    internal class Serializer
    {
        public static ScopedSerializer Global { get; } = new ScopedSerializer();

        public static T ReadJson<T>(ReadOnlyBuffer<byte> data) where T : class, new() => Global.ReadJson<T>(data);
        public static void WriteJson<T>(ArrayFormatter data, T obj) where T : class, new() => Global.WriteJson(data, obj);

        public static ScopedSerializer CreateScope() => new ScopedSerializer();
    }

    internal class ScopedSerializer
    {
        private readonly AsyncEvent<Func<Exception, Task>> _errorEvent = new AsyncEvent<Func<Exception, Task>>();
        public event Func<Exception, Task> Error
        {
            add { _errorEvent.Add(value); }
            remove { _errorEvent.Remove(value); }
        }

        public T ReadJson<T>(ReadOnlyBuffer<byte> data)
            where T : class, new()
            => ModelMap.For<T>().ReadJson(new JsonReader(data.Span, SymbolTable.InvariantUtf8));
        public void WriteJson<T>(ArrayFormatter data, T obj)
            where T : class, new()
            => ModelMap.For<T>().WriteJson(obj, new JsonWriter(data));
    }
}
