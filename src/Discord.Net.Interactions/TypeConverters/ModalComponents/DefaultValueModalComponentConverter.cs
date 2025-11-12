using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions;

internal sealed class DefaultValueModalComponentConverter<T> : ModalComponentTypeConverter<T>
    where T : IConvertible
{
    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        try
        {
            return option.Type switch
            {
                ComponentType.SelectMenu when option.Values.Count == 1 => Task.FromResult(TypeConverterResult.FromSuccess(Convert.ChangeType(option.Values.First(), typeof(T)))),
                ComponentType.TextInput => Task.FromResult(TypeConverterResult.FromSuccess(Convert.ChangeType(option.Value, typeof(T)))),
                _ => Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"{option.Type} doesn't have a convertible value."))
            };
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException)
        {
            return Task.FromResult(TypeConverterResult.FromError(ex));
        }
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, InputComponentInfo component, object value)
    {
        var strValue = Convert.ToString(value);

        switch (builder)
        {
            case TextInputBuilder textInput:
                textInput.WithValue(strValue);
                break;
            case SelectMenuBuilder selectMenu when component.ComponentType is ComponentType.SelectMenu:
                selectMenu.Options.FirstOrDefault(x => x.Value == strValue)?.IsDefault = true;
                break;
            default:
                throw new InvalidOperationException($"{typeof(IConvertible).Name}s cannot be used to populate components other than SelectMenu and TextInput.");
        }
        ;

        return Task.CompletedTask;
    }
}
