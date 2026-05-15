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
                ComponentType.SelectMenu => Success(Convert.ChangeType(option.Values.First(), typeof(T))),
                ComponentType.TextInput => Success(Convert.ChangeType(option.Value, typeof(T))),
                ComponentType.CheckboxGroup => Success(Convert.ChangeType(option.Values.First(), typeof(T))),
                ComponentType.RadioGroup => Success(Convert.ChangeType(option.Value, typeof(T))),
                ComponentType.Checkbox => Success(Convert.ChangeType(option.BoolValue, typeof(T))),
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
        if (value is null)
            return Task.CompletedTask;

        var strValue = Convert.ToString(value);

        switch (builder)
        {
            case TextInputBuilder textInput when strValue != string.Empty:
                textInput.WithValue(strValue);
                break;
            case SelectMenuBuilder selectMenu when strValue != string.Empty:
                foreach (var option in selectMenu.Options)
                {
                    option.IsDefault = option.Value == strValue;
                }
                break;
            case CheckboxBuilder checkbox when value is bool boolValue:
                checkbox.DefaultState = boolValue;
                break;
            case CheckboxGroupBuilder checkboxGroup when strValue != string.Empty:
                checkboxGroup.Options =
                    checkboxGroup.Options.Select(x =>
                    {
                        x.DefaultState = x.Value == strValue;
                        return x;
                    }).ToList();
                break;
            case RadioGroupBuilder radioGroup when strValue != string.Empty:
                radioGroup.Options = radioGroup.Options.Select(x =>
                {
                    x.IsDefault = x.Value == strValue;
                    return x;
                }).ToList();
                break;
        };

        return Task.CompletedTask;
    }

    private Task<TypeConverterResult> Success<TResult>(TResult result)
        => Task.FromResult(TypeConverterResult.FromSuccess(result));
}
