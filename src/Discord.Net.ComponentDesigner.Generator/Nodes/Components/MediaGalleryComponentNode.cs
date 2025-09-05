using Discord.ComponentDesigner.Generator.Parser;
using System.Collections.Generic;
using System.Linq;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesigner.Generator.Nodes;

public sealed class MediaGalleryComponentNode : ComponentNode
{
    public override string FriendlyName => "Media Gallery";

    public override NodeKind Kind => NodeKind.MediaGallery;

    public IReadOnlyList<MediaGalleryItem> Items { get; }

    public MediaGalleryComponentNode(CXmlElement xml, ComponentNodeContext context, bool mapId = true) : base(xml,
        context, mapId)
    {
        var items = new List<MediaGalleryItem>();

        foreach (var childXml in xml.Children)
        {
            if (childXml is not CXmlElement element)
            {
                context.ReportDiagnostic(
                    Diagnostics.InvalidChildNodeType,
                    context.GetLocation(childXml),
                    FriendlyName,
                    "text"
                );

                continue;
            }

            if (element.Name.Value is not "item")
            {
                context.ReportDiagnostic(
                    Diagnostics.InvalidChildComponentType,
                    context.GetLocation(childXml),
                    FriendlyName,
                    element.Name.Value
                );
            }

            items.Add(new MediaGalleryItem(element, context));
        }

        Items = items;
    }

    public override void ReportValidationErrors()
    {
        base.ReportValidationErrors();

        foreach (var item in Items) item.ReportValidationErrors();

        if (Items.Count is 0)
        {
            Context.ReportDiagnostic(
                Diagnostics.EmptyMediaGallery,
                Location
            );
        }

        if (Items.Count > Constants.MAX_MEDIA_ITEMS)
        {
            Context.ReportDiagnostic(
                Diagnostics.TooManyMediaGalleryItems,
                Location
            );
        }
    }

    public override string Render()
        => $"""
            new {Context.KnownTypes.MediaGalleryBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                {
                    string.Join(
                        ",\n".Postfix(4),
                        Items.Select(x => x.Render().WithNewlinePadding(4))
                    )
                }
            )
            """;
}

public sealed class MediaGalleryItem : ComponentNode
{
    public override string FriendlyName => "Media Gallery Item";
    public override NodeKind Kind => NodeKind.MediaGalleryItem;
    public ComponentProperty<string> Url { get; }

    public ComponentProperty<string> Description { get; }

    public ComponentProperty<bool> IsSpoiler { get; }

    public MediaGalleryItem(CXmlElement xml, ComponentNodeContext context, bool mapId = true) : base(xml, context,
        mapId)
    {
        Url = MapProperty("url");

        Description = MapProperty(
            "description",
            optional: true,
            validators: [Validators.LengthBounds(upper: Constants.MAX_MEDIA_ITEM_DESCRIPTION_LENGTH)]
        );

        IsSpoiler = MapProperty<bool>("spoiler", ParseBooleanProperty, optional: true);
    }


    public override string Render()
        => $"""
            new {Context.KnownTypes.MediaGalleryItemPropertiesType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                media: new {Context.KnownTypes.UnfurledMediaItemPropertiesType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}({Url}),
                description: {Description.ToString().WithNewlinePadding(4)},
                isSpoiler: {IsSpoiler}
            )
            """;
}
