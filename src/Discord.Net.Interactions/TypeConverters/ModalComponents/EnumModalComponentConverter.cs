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
        if (option.Type is not ComponentType.SelectMenu and not ComponentType.TextInput)
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"{option.Type} input type cannot be converted to {typeof(T).FullName}"));

        var value = option.Type switch
        {
            ComponentType.SelectMenu => string.Join(",", option.Values),
            ComponentType.TextInput => option.Value,
            _ => null
        };

        if (Enum.TryParse<T>(value, out var result))
            return Task.FromResult(TypeConverterResult.FromSuccess(result));

        return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Value {option.Value} cannot be converted to {typeof(T).FullName}"));
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
    {
        if (builder is not SelectMenuBuilder selectMenu || component.ComponentType is not ComponentType.SelectMenu)
            throw new InvalidOperationException($"{nameof(EnumModalComponentConverter<T>)} can only write to select menu components.");

        if (selectMenu.MaxValues > 1 && !_isFlags)
            throw new InvalidOperationException($"Enum type {typeof(T).FullName} is not a [Flags] enum, so it cannot be used in a multi-select menu.");

        var visibleOptions = _options.Where(x => !x.Predicate?.Invoke(interaction) ?? true);

        foreach (var option in visibleOptions)
        {
            var optionBuilder = new SelectMenuOptionBuilder(option.MenuOption);

            if(value is T enumValue && option.Value is T optionValue)
                optionBuilder.IsDefault = _isFlags ? enumValue.HasFlag(optionValue) : enumValue.Equals(option.Value);

            selectMenu.AddOption(optionBuilder);
        }

        return Task.CompletedTask;
    }
}
