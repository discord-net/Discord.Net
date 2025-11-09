using System;
using System.Threading.Tasks;

namespace Discord.Interactions.TypeConverters.ModalInputs;
public abstract class ModalComponentTypeConverter : ITypeConverter<IComponentInteractionData>
{
    public abstract bool CanConvertTo(Type type);

    public abstract Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services);

    public virtual Task WriteAsync<TBuilder>(TBuilder builder, InputComponentInfo component, object value)
        where TBuilder : class, IInteractableComponentBuilder
        => Task.CompletedTask;
}

public abstract class ModalComponentTypeConverter<T> : ModalComponentTypeConverter
{
    /// <inheritdoc/>
    public sealed override bool CanConvertTo(Type type) =>
        typeof(T).IsAssignableFrom(type);
}
