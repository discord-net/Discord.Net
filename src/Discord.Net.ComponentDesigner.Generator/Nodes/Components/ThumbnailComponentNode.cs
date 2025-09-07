using Discord.ComponentDesignerGenerator.Parser;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesignerGenerator.Nodes;

public sealed class ThumbnailComponentNode : ComponentNode
{
    public override string FriendlyName => "Thumbnail";
    public override NodeKind Kind => NodeKind.Thumbnail;
    public ComponentProperty<string> Url { get; }

    public ComponentProperty<string> Description { get; }

    public ComponentProperty<bool> IsSpoiler { get; }

    public ThumbnailComponentNode(CXmlElement xml, ComponentNodeContext context) : base(xml, context)
    {
        Url = MapProperty("url");

        Description = MapProperty(
            "description",
            optional: true,
            validators: [Validators.LengthBounds(upper: Constants.THUMBNAIL_DESCRIPTION_MAX_LENGTH)]
        );

        IsSpoiler = MapProperty<bool>("spoiler", ValueParsers.ParseBooleanProperty, optional: true);
    }

    public override string Render()
        => $"""
            new {Context.KnownTypes.ThumbnailBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                media: {Context.KnownTypes.UnfurledMediaItemPropertiesType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}({Url.ToString().WithNewlinePadding(4)}),
                description: {Description.ToString().WithNewlinePadding(4)},
                isSpoiler: {IsSpoiler}
            )
            """;
}
