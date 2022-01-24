using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal class ArrayReader<T> : TypeReader where T : IEnumerable
    {
        private readonly TypeReader _baseReader;

        public ArrayReader(InteractionService interactionService)
        {
            if()

            interactionService.GetTypeReader(typeof)
        }

        public override TypeReaderTarget[] TypeReaderTargets { get; }

        public override bool CanConvertTo(Type type) => throw new NotImplementedException();

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, object input, IServiceProvider services)
        {
            if(input is IEnumerable enumerable)
                return Task.FromResult(TypeConverterResult.FromSuccess(new ))
        }
    }
}
