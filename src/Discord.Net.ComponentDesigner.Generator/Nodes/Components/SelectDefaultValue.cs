using Discord.ComponentDesigner.Generator.Parser;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesigner.Generator.Nodes;

public sealed class SelectDefaultValue : ComponentNode
{
    public override string FriendlyName => "Select Default Value";

    public override NodeKind Kind => NodeKind.SelectDefault;

    public new ComponentProperty<ulong> Id { get; }

    public ComponentProperty<SelectDefaultValueType> Type { get; }

    public SelectDefaultValue(CXmlElement xml, ComponentNodeContext context) : base(xml, context, mapId: false)
    {
        Id = MapProperty<ulong>(
            "id",
            ParseSnowflakeProperty
        );

        Type = MapProperty<SelectDefaultValueType>(
            "type",
            ParseEnumProperty<SelectDefaultValueType>,
            optional: true
        );
    }

    public override string Render()
        => $"""
            new {Context.KnownTypes.SelectMenuDefaultValueType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                id: {Id},
                type: {Context.KnownTypes.SelectDefaultValueTypeEnumType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{Type}
            )
            """;
}

public enum SelectDefaultValueType
{
    User,
    Role,
    Channel
}
