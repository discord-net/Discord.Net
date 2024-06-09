using Discord.Models.Json;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Discord.Converters;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ComponentTypeAttribute(uint type) : Attribute
{
    public readonly uint Type = type;
}

public sealed class ComponentConverter : TypeUnionConverter<MessageComponent, uint>
{
    private static readonly Dictionary<uint, Type> ComponentTypes;
    protected override bool UseGenericAsDefault { get; } = false;
    protected override string UnionDelimiterName { get; } = "type";

    static ComponentConverter()
    {
        ComponentTypes = typeof(MessageComponent).Assembly
            .GetTypes()
            .Where(x =>
                x.IsClass &&
                x != typeof(MessageComponent) &&
                x.IsAssignableTo(typeof(MessageComponent)) &&
                x.GetCustomAttribute<ComponentTypeAttribute>() is not null
            )
            .ToDictionary(x => x.GetCustomAttribute<ComponentTypeAttribute>()!.Type, x => x);
    }

    protected override bool TryGetTypeFromDelimiter(uint delimiter, [MaybeNullWhen(false)] out Type type)
        => ComponentTypes.TryGetValue(delimiter, out type);
}
