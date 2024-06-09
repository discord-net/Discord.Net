using Discord.Models.Json;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Discord.Converters;

[AttributeUsage(AttributeTargets.Class)]
public sealed class InteractionDataTypeAttribute(uint type) : Attribute
{
    public readonly uint Type = type;
}

public sealed class InteractionDataConverter : TypeUnionConverter<InteractionData, uint>
{
    private static readonly Dictionary<uint, Type> InteractionDataTypes;

    protected override bool UseGenericAsDefault => false;
    protected override string UnionDelimiterName => "type";

    static InteractionDataConverter()
    {
        InteractionDataTypes = typeof(InteractionData).Assembly
            .GetTypes()
            .Where(x =>
                x.IsClass &&
                x != typeof(InteractionData) &&
                x.IsAssignableTo(typeof(InteractionData)) &&
                x.GetCustomAttribute<InteractionDataTypeAttribute>() is not null
            )
            .ToDictionary(x => x.GetCustomAttribute<InteractionDataTypeAttribute>()!.Type, x => x);
    }

    protected override bool TryGetTypeFromDelimiter(uint delimiter, [MaybeNullWhen(false)] out Type type)
        => InteractionDataTypes.TryGetValue(delimiter, out type);
}
