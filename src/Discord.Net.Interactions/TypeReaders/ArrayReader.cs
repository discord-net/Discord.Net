using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal class ArrayReader<T> : TypeReader<T> where T : IEnumerable
    {
        private readonly TypeReader _baseReader;

        public ArrayReader(InteractionService interactionService)
        {
            if()

            interactionService.GetTypeReader(typeof)
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, object input, IServiceProvider services)
        {
            if(input is IEnumerable enumerable)
                return Task.FromResult(TypeConverterResult.FromSuccess(new ))
        }
    }
}
