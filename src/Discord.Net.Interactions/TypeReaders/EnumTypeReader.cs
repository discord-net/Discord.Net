using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal sealed class EnumTypeReader<T> : TypeReader<T>
        where T : struct, Enum
    {
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, string option, IServiceProvider services)
        {
            if (Enum.TryParse<T>(option, out var result))
                return Task.FromResult(TypeConverterResult.FromSuccess(result));
            else
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Value {option} cannot be converted to {nameof(T)}"));
        }

        public override Task<string> SerializeAsync(object obj)
        {
            var name = Enum.GetName(typeof(T), obj);

            if (name is null)
                throw new ArgumentException($"Enum name cannot be parsed from {obj}");

            return Task.FromResult(name);
        }
    }
}
