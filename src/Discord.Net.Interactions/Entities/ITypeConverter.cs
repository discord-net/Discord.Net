using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal interface ITypeConverter<T>
    {
        public bool CanConvertTo(Type type);

        public Task<TypeConverterResult> ReadAsync(IInteractionContext context, T option, IServiceProvider services);
    }
}
