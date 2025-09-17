using Discord.Interactions.TypeConverters.ModalInputs;
using System;
using System.Threading.Tasks;

namespace Discord.Interactions.TypeConverters.ModalComponents;

internal class NullableModalComponentConverter<T> : ModalComponentTypeConverter<T>
{
    private readonly ModalComponentTypeConverter _typeConverter;

    public NullableModalComponentConverter(InteractionService interactionService, IServiceProvider services)
    {
        var type = Nullable.GetUnderlyingType(typeof(T));

        if (type is null)
            throw new ArgumentException($"No type {nameof(TypeConverter)} is defined for this {type.FullName}", "type");

        _typeConverter = interactionService.GetModalInputTypeConverter(type, services);
    }

    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
        => string.IsNullOrEmpty(option.Value) ? Task.FromResult(TypeConverterResult.FromSuccess(null)) : _typeConverter.ReadAsync(context, option, services);

    public override Task WriteAsync<TBuilder>(TBuilder builder, InputComponentInfo component, object value)
        => _typeConverter.WriteAsync(builder, component, value);
}
