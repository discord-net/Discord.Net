using Discord.Interactions.Utilities;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Interactions;

internal sealed class EnumModalComponentConverter<T> : ModalComponentTypeConverter<T>
    where T : struct, Enum
{
    private readonly bool _isFlags;
    private readonly ImmutableArray<EnumSelectMenuOption> _options;

    public EnumModalComponentConverter()
    {
        _isFlags = typeof(T).IsDefined(typeof(FlagsAttribute));
        _options = EnumUtils.BuildSelectMenuOptions(typeof(T)).ToImmutableArray();
    }

    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (option.Type is not ComponentType.SelectMenu and not ComponentType.TextInput and not ComponentType.CheckboxGroup and not ComponentType.RadioGroup)
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"{option.Type} input type cannot be converted to {typeof(T).FullName}"));

        var value = option.Type switch
        {
            ComponentType.SelectMenu or ComponentType.CheckboxGroup => string.Join(",", option.Values),
            ComponentType.TextInput or ComponentType.RadioGroup => option.Value,
            _ => null
        };

        if (Enum.TryParse<T>(value, out var result))
            return Task.FromResult(TypeConverterResult.FromSuccess(result));

        return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Value {option.Value} cannot be converted to {typeof(T).FullName}"));
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
    {
        return (builder, component.ComponentType) switch
        {
            (SelectMenuBuilder selectMenu, ComponentType.SelectMenu) => WriteSelectMenuAsync(selectMenu, interaction, component, value),
            (CheckboxGroupBuilder checkboxGroup, ComponentType.CheckboxGroup) => WriteCheckboxGroupAsync(checkboxGroup, interaction, component, value),
            (RadioGroupBuilder radioGroup, ComponentType.RadioGroup) => WriteRadioGroupAsync(radioGroup, interaction, component, value),
            _ => throw new InvalidOperationException($"Default enum converter for modal components can only be used with select-menu, checkbox-group, and checkbox.")
        };
    }

    private Task WriteSelectMenuAsync(SelectMenuBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
    {
        if (builder.MaxValues > 1 && !_isFlags)
            throw new InvalidOperationException(
                $"Enum type {typeof(T).FullName} is not a [Flags] enum, so it cannot be used in a multi-select menu.");

        var visibleOptions = _options.Where(x => !x.Predicate?.Invoke(interaction) ?? true);

        foreach (var option in visibleOptions)
        {
            var optionBuilder = option.ToSelectMenuOptionBuilder();

            if(value is T enumValue && option.Value is T optionValue)
                optionBuilder.IsDefault = _isFlags ? enumValue.HasFlag(optionValue) : enumValue.Equals(option.Value);

            builder.AddOption(optionBuilder);
        }

        if (builder.MaxValues < builder.Options.Count(x => x.IsDefault ?? false))
            throw new InvalidOperationException(
                "Select-menu cannot have more default selected values than the maximum amount allowed.");

        return Task.CompletedTask;
    }

    private Task WriteCheckboxGroupAsync(CheckboxGroupBuilder builder, IDiscordInteraction interaction,  InputComponentInfo component, object value)
    {
        if (builder.MaxValues > 1 && !_isFlags)
            throw new InvalidOperationException(
                $"Enum type {typeof(T).FullName} is not a [Flags] enum, so it cannot be used in a multi-select checkbox group.");

        var visibleOptions = _options.Where(x => !x.Predicate?.Invoke(interaction) ?? true);

        foreach (var option in visibleOptions)
        {
            var optionBuilder = option.ToCheckboxGroupOptionProperties();

            if(value is T enumValue && option.Value is T optionValue)
                optionBuilder.DefaultState = _isFlags ? enumValue.HasFlag(optionValue) : enumValue.Equals(option.Value);

            builder.AddOption(optionBuilder);
        }

        if (builder.MaxValues < builder.Options.Count(x => x.DefaultState ?? false))
            throw new InvalidOperationException(
                "Checkbox-group cannot have more default selected values than the maximum amount allowed.");

        return Task.CompletedTask;
    }

    private Task WriteRadioGroupAsync(RadioGroupBuilder builder, IDiscordInteraction interaction,  InputComponentInfo component, object value)
    {
        var visibleOptions = _options.Where(x => !x.Predicate?.Invoke(interaction) ?? true);

        foreach (var option in visibleOptions)
        {
            var optionBuilder = option.ToRadioGroupOptionProperties();

            if(value is T enumValue && option.Value is T optionValue)
                optionBuilder.IsDefault = _isFlags ? enumValue.HasFlag(optionValue) : enumValue.Equals(option.Value);

            builder.AddOption(optionBuilder);
        }

        if (1 < builder.Options.Count(x => x.IsDefault ?? false))
            throw new InvalidOperationException(
                "Radio-group cannot have more default selected values than the maximum amount allowed.");

        return Task.CompletedTask;
    }
}
