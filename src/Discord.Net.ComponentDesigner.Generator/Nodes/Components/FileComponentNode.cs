using Discord.ComponentDesigner.Generator.Parser;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesigner.Generator.Nodes;

public sealed class FileComponentNode : ComponentNode
{
    public override string FriendlyName => "File";
    public override NodeKind Kind => NodeKind.File;

    public ComponentProperty<string> Url { get; }

    public ComponentProperty<bool> IsSpoiler { get; }

    public FileComponentNode(CXmlElement xml, ComponentNodeContext context, bool mapId = true) : base(xml, context, mapId)
    {
        Url = MapProperty("url");
        IsSpoiler = MapProperty<bool>("spoiler", ValueParsers.ParseBooleanProperty, optional: true);
    }

    public override string Render()
        => $"""
            new {Context.KnownTypes.FileComponentBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                media: new {Context.KnownTypes.UnfurledMediaItemPropertiesType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}({Url}),
                isSpoiler: {IsSpoiler},
                id: {Id}
            )
            """;
}
