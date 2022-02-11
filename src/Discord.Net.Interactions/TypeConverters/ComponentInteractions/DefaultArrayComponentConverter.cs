using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal sealed class DefaultArrayComponentConverter<T> : ComponentTypeConverter<T>
    {
        private readonly TypeReader _typeReader;
        private readonly Type _underlyingType;

        public DefaultArrayComponentConverter(InteractionService interactionService)
        {
            var type = typeof(T);

            if (!type.IsArray)
                throw new InvalidOperationException($"{nameof(DefaultArrayComponentConverter<T>)} cannot be used to convert a non-array type.");

            _underlyingType = typeof(T).GetElementType();
            _typeReader = interactionService.GetTypeReader(_underlyingType);
        }

        public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
        {
            var results = new List<TypeConverterResult>();

            foreach (var value in option.Values)
            {
                var result = await _typeReader.ReadAsync(context, value, services).ConfigureAwait(false);

                if (!result.IsSuccess)
                    return result;

                results.Add(result);
            }

            var destination = Array.CreateInstance(_underlyingType, results.Count);

            for (var i = 0; i < results.Count; i++)
                destination.SetValue(results[i].Value, i);

            return TypeConverterResult.FromSuccess(destination);
        }
    }
}
