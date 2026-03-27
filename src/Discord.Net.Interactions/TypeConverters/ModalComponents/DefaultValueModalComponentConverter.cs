using System;
using System.Collections.Generic;
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
                ComponentType.SelectMenu when option.Values.Count == 1 => Success(Convert.ChangeType(option.Values.First(), typeof(T))),
                ComponentType.TextInput => Success(Convert.ChangeType(option.Value, typeof(T))),
                ComponentType.CheckboxGroup when option.Values.Count == 1 => Success(Convert.ChangeType(option.Value, typeof(T))),
                ComponentType.RadioGroup => Success(Convert.ChangeType(option.Value, typeof(T))),
                _ => Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"{option.Type} doesn't have a convertible value."))
            };
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException)
        {
            return Task.FromResult(TypeConverterResult.FromError(ex));
        }
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
    {
        var strValue = Convert.ToString(value);

        if(string.IsNullOrEmpty(strValue))
            return Task.CompletedTask;

        switch (builder)
        {
            case TextInputBuilder textInput:
                textInput.WithValue(strValue);
                break;
            case SelectMenuBuilder selectMenu when component.ComponentType is ComponentType.SelectMenu:
                foreach (var option in selectMenu.Options)
                {
                    option.IsDefault = option.Value == strValue;
                }
                break;
            case CheckboxBuilder checkbox when value is bool boolValue:
                checkbox.DefaultState = boolValue;
                break;
            case CheckboxGroupBuilder checkboxGroup when component.ComponentType is ComponentType.CheckboxGroup:
                checkboxGroup.Options =
                    checkboxGroup.Options.Select(x =>
                    {
                        x.DefaultState = x.Value == strValue;
                        return x;
                    }).ToList();
                break;
            case RadioGroupBuilder radioGroup:
                radioGroup.Options = radioGroup.Options.Select(x =>
                {
                    x.IsDefault = x.Value == strValue;
                    return x;
                }).ToList();
                break;
            default:
                throw new InvalidOperationException($"{nameof(IConvertible)}s cannot be used to populate components other than SelectMenu and TextInput.");
        }
        ;

        return Task.CompletedTask;
    }

    private Task<TypeConverterResult> Success<TResult>(TResult result)
        => Task.FromResult(TypeConverterResult.FromSuccess(result));
}
