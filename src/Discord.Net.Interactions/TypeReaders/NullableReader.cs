using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal class NullableReader<T> : TypeReader<T>
    {
        private readonly TypeReader _typeReader;

        public NullableReader(InteractionService interactionService, IServiceProvider services)
        {
            var type = Nullable.GetUnderlyingType(typeof(T));

            if (type is null)
                throw new ArgumentException($"No type {nameof(TypeConverter)} is defined for this {type.FullName}", "type");

            _typeReader = interactionService.GetTypeReader(type, services);
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, string option, IServiceProvider services)
            => string.IsNullOrEmpty(option) ? Task.FromResult(TypeConverterResult.FromSuccess(null)) : _typeReader.ReadAsync(context, option, services);
    }
}
