using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal class NullableComponentConverter<T> : ComponentTypeConverter<T>
    {
        private readonly ComponentTypeConverter _typeConverter;

        public NullableComponentConverter(InteractionService interactionService, IServiceProvider services)
        {
            var type = Nullable.GetUnderlyingType(typeof(T));

            if (type is null)
                throw new ArgumentException($"No type {nameof(TypeConverter)} is defined for this {type.FullName}", "type");

            _typeConverter = interactionService.GetComponentTypeConverter(type, services);
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
            => string.IsNullOrEmpty(option.Value) ? Task.FromResult(TypeConverterResult.FromSuccess(null)) : _typeConverter.ReadAsync(context, option, services);
    }
}
