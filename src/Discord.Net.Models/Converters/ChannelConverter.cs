using Discord.API;
using Discord.Models.Json;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ChannelTypeOfAttribute(uint type) : Attribute
{
    public readonly uint Type = type;
}

public class ChannelConverter : TypeUnionConverter<ChannelModel, uint>
{
    protected override bool UseGenericAsDefault => true;
    protected override string UnionDelimiterName => "type";

    private static readonly Dictionary<uint, Type> Channels;

    static ChannelConverter()
    {
        Channels = typeof(ChannelModel).Assembly.GetTypes()
            .Where(x =>
                x.IsClass && x.IsAssignableTo(typeof(ChannelModel)) && x != typeof(ChannelModel) &&
                x.GetCustomAttribute<ChannelTypeOfAttribute>() is not null)
            .ToDictionary(x => x.GetCustomAttribute<ChannelTypeOfAttribute>()!.Type, x => x);
    }

    protected override bool TryGetTypeFromDelimiter(uint delimiter, [MaybeNullWhen(false)]  out Type type)
        => Channels.TryGetValue(delimiter, out type);
}
