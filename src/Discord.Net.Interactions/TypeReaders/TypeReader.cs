using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    public abstract class TypeReader : ITypeConverter<string>
    {
        public abstract bool CanConvertTo(Type type);
        public abstract Task<TypeConverterResult> ReadAsync(IInteractionContext context, string option, IServiceProvider services);
        public virtual Task<string> SerializeAsync(object obj) => Task.FromResult(obj.ToString());
    }

    public abstract class TypeReader<T> : TypeReader
    {
        public sealed override bool CanConvertTo(Type type) =>
            typeof(T).IsAssignableFrom(type);
    }
}
