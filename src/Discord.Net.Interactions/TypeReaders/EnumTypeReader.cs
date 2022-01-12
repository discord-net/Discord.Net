using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal class EnumTypeReader<T> : TypeReader<T> where T : struct, Enum
    {
        /// <inheritdoc />
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, object input, IServiceProvider services)
        {
            if (Enum.TryParse<T>(input as string, out var result))
                return Task.FromResult(TypeConverterResult.FromSuccess(result));
            else
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Value {input} cannot be converted to {nameof(T)}"));
        }

        public override string Serialize(object value)
        {
            if (value is not Enum)
                throw new ArgumentException($"{value} isn't an {nameof(Enum)}.", nameof(value));

            return value.ToString();
        }
    }
}
