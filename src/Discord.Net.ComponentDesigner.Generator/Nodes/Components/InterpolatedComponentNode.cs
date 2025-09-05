using Discord.ComponentDesigner.Generator.Parser;
using Microsoft.CodeAnalysis;

namespace Discord.ComponentDesigner.Generator.Nodes;

public sealed class InterpolatedComponentNode : ComponentNode
{
    public override NodeKind Kind { get; }

    public InterpolationInfo InterpolationInfo { get; }

    private readonly CXmlValue.Interpolation _value;
    private readonly string _preferredType;

    public InterpolatedComponentNode(
        CXmlValue.Interpolation xml,
        ComponentNodeContext context,
        NodeKind kind,
        string? preferredType = null
    ) : base(xml, context)
    {
        _value = xml;

        InterpolationInfo = context.Interpolations[_value.InterpolationIndex];

        _preferredType = preferredType ??
                         InterpolationInfo.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        Kind = NodeKind.Interpolated | kind;
    }

    public InterpolatedComponentNode(
        CXmlValue.Interpolation xml,
        ComponentNodeContext context,
        string? preferredType = null
    ) : this(
        xml,
        context,
        context.Interpolations[xml.InterpolationIndex].Type.ToNodeKind(context.KnownTypes),
        preferredType
    )
    {
    }

    public override string FriendlyName => $"Interpolation[{_value.InterpolationIndex}]";

    public override string Render()
        => $"designer.GetValue<{_preferredType}>({_value.InterpolationIndex})";
}
