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
    private readonly ImmutableArray<(SelectMenuOptionBuilder Option, Predicate<IDiscordInteraction> Predicate)> _options;

    public EnumModalComponentConverter()
    {
        var names = Enum.GetNames(typeof(T));
        var members = names.SelectMany(x => typeof(T).GetMember(x));

        _isFlags = typeof(T).GetCustomAttribute<FlagsAttribute>() is not null;

        _options = members.Select(x =>
        {
            var selectMenuOptionAttr = x.GetCustomAttribute<SelectMenuOptionAttribute>();
            var hideAttr = x.GetCustomAttribute<HideAttribute>();
            Predicate<IDiscordInteraction> predicate = hideAttr != null ? hideAttr.Predicate : null;
            return (new SelectMenuOptionBuilder(x.GetCustomAttribute<ChoiceDisplayAttribute>()?.Name ?? x.Name, x.Name, selectMenuOptionAttr?.Description, selectMenuOptionAttr?.Emote != null ? Emote.Parse(selectMenuOptionAttr?.Emote) : null, selectMenuOptionAttr?.IsDefault), predicate);
        }).ToImmutableArray();
    }

    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (option.Type is not ComponentType.SelectMenu or ComponentType.TextInput)
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

        selectMenu.WithOptions([.. _options.Where(x => !x.Predicate?.Invoke(interaction) ?? true).Select(x => x.Option)]);

        return Task.CompletedTask;
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class SelectMenuOptionAttribute : Attribute
{
    public string Description { get; set; }

    public bool IsDefault { get; set; }

    public string Emote { get; set; }
}
