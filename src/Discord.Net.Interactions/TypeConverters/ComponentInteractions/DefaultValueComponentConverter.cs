using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Interactions.TypeConverters
{
    internal sealed class DefaultValueComponentConverter<T> : ComponentTypeConverter<T>
        where T : class, IConvertible
    {
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
        {
            return Task.FromResult(TypeConverterResult.FromSuccess(Convert.ChangeType(option.Values, TypeCode.Object)));
        }
    }
}
