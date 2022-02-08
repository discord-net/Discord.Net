using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            return TypeConverterResult.FromSuccess(results.Select(x => x.Value).ToArray());
        }
    }
}
