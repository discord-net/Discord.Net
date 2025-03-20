using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal sealed class DefaultValueComponentConverter<T> : ComponentTypeConverter<T>
        where T : IConvertible
    {
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
        {
            try
            {
                return option.Type switch
                {
                    ComponentType.SelectMenu => Task.FromResult(TypeConverterResult.FromSuccess(Convert.ChangeType(string.Join(",", option.Values), typeof(T)))),
                    ComponentType.TextInput => Task.FromResult(TypeConverterResult.FromSuccess(Convert.ChangeType(option.Value, typeof(T)))),
                    _ => Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"{option.Type} doesn't have a convertible value."))
                };
            }
            catch (Exception ex) when (ex is FormatException or InvalidCastException)
            {
                return Task.FromResult(TypeConverterResult.FromError(ex));
            }
        }
    }
}
