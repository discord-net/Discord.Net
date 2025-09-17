using Discord.Interactions.TypeConverters.ModalInputs;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Interactions.TypeConverters.ModalComponents;

internal sealed class EnumModalComponentConverter<T> : ModalComponentTypeConverter<T>
    where T : struct, Enum
{
    private readonly bool _isFlags;
    private readonly ImmutableArray<SelectMenuOptionBuilder> _options;

    public EnumModalComponentConverter()
    {
        var names = Enum.GetNames(typeof(T));
        var members = names.SelectMany(x => typeof(T).GetMember(x)).Where(x => !x.IsDefined(typeof(HideAttribute), true));

        if (members.Count() > SelectMenuBuilder.MaxOptionCount)
            throw new InvalidOperationException($"Enum type {typeof(T).FullName} has too many visible members to be used in a select menu. Maximum visible members is {SelectMenuBuilder.MaxOptionCount}, but {members.Count()} are visible.");

        _isFlags = typeof(T).GetCustomAttribute<FlagsAttribute>() is not null;

        _options = members.Select(x =>
        {
            var selectMenuOptionAttr = x.GetCustomAttribute<SelectMenuOptionAttribute>();
            return new SelectMenuOptionBuilder(x.GetCustomAttribute<ChoiceDisplayAttribute>()?.Name ?? x.Name, x.Name, selectMenuOptionAttr?.Description, Emote.Parse(selectMenuOptionAttr?.Emote), selectMenuOptionAttr?.IsDefault);
        }).ToImmutableArray();
    }

    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if(option.Type is not ComponentType.SelectMenu or ComponentType.TextInput)
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"{option.Type} input type cannot be converted to {typeof(T).FullName}"));

        var value = option.Type switch
        {
            ComponentType.SelectMenu => string.Join(",", option.Values),
            ComponentType.TextInput => option.Value,
            _ => null
        };

        if(Enum.TryParse<T>(value, out var result))
            return Task.FromResult(TypeConverterResult.FromSuccess(result));

        return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Value {option.Value} cannot be converted to {typeof(T).FullName}"));
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, InputComponentInfo component, object value)
    {
        if (builder is not SelectMenuBuilder selectMenu || component.ComponentType is not ComponentType.SelectMenu)
            throw new InvalidOperationException($"{nameof(EnumModalComponentConverter<T>)} can only write to select menu components.");

        if(selectMenu.MaxValues > 1 && !_isFlags)
            throw new InvalidOperationException($"Enum type {typeof(T).FullName} is not a [Flags] enum, so it cannot be used in a multi-select menu.");

        selectMenu.WithOptions(_options.ToList());

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
