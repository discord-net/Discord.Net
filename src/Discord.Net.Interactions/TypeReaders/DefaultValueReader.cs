using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal sealed class DefaultValueReader<T> : TypeReader<T>
        where T : IConvertible
    {
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, string option, IServiceProvider services)
        {
            try
            {
                var converted = Convert.ChangeType(option, typeof(T));
                return Task.FromResult(TypeConverterResult.FromSuccess(converted));
            }
            catch (InvalidCastException castEx)
            {
                return Task.FromResult(TypeConverterResult.FromError(castEx));
            }
        }
    }
}
