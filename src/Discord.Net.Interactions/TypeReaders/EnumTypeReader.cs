using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal class EnumTypeReader<T> : TypeReader<T> where T : struct, Enum
    {
        /// <inheritdoc />
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, string input, IServiceProvider services)
        {
            if (Enum.TryParse<T>(input, out var result))
                return Task.FromResult(TypeConverterResult.FromSuccess(result));
            else
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Value {input} cannot be converted to {nameof(T)}"));
        }

        public override string Serialize(object value) => value.ToString();
    }
}
