using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    public abstract class ComponentTypeConverter : ITypeConverter<IComponentInteractionData>
    {
        public abstract bool CanConvertTo(Type type);
        public abstract Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services);
        public virtual Task<string> SerializeAsync(object obj) => Task.FromResult(obj.ToString());
    }

    public abstract class ComponentTypeConverter<T> : ComponentTypeConverter
    {
        public sealed override bool CanConvertTo(Type type) =>
            typeof(T).IsAssignableFrom(type);
    }
}
