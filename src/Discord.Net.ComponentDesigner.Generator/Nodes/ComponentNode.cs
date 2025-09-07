using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.ComponentDesignerGenerator.Nodes;

public abstract class ComponentNode
{
    public abstract string FriendlyName { get; }

    public abstract NodeKind Kind { get; }

    public ComponentProperty<int>? Id { get; }

    public CXmlElement? Element => _cxml as CXmlElement;

    public ComponentNodeContext Context { get; }

    public Location Location => Context.GetLocation(_cxml);

    private readonly List<IComponentProperty> _properties = [];
    private readonly HashSet<string> _consumedProperties = [];

    private ICXml _cxml;

    protected ComponentNode(CXmlElement xml, ComponentNodeContext context, bool mapId = true) : this((ICXml) xml,
        context)
    {
        if (mapId)
            Id = MapProperty<int>("id", ValueParsers.ParseIntProperty, optional: true);
    }

    protected ComponentNode(ICXml xml, ComponentNodeContext context)
    {
        _cxml = xml;
        Context = context;
    }

    public static ComponentNode? Create(ICXml? xml, ComponentNodeContext context)
    {
        if (xml is CXmlElement element) return Create(element, context);

        if (xml is CXmlValue.Interpolation interpolated)
            return new InterpolatedComponentNode(interpolated, context);

        return null;
    }

    public static ComponentNode? Create(CXmlElement? xml, ComponentNodeContext context)
    {
        if (xml is null) return null;

        switch (xml.Name.Value)
        {
            case "row" or "actionrow":
                return new ActionRowComponentNode(xml, context);

            case "button":
                return new ButtonComponentNode(xml, context);

            case "stringselect":
                return new StringSelectComponentNode(xml, context);

            case "textinput":
                return new TextInputComponentNode(xml, context);

            case "userselect":
                return new UserSelectComponentNode(xml, context);

            case "roleselect":
                return new RoleSelectComponentNode(xml, context);

            case "mentionableselect":
                return new MentionableSelectComponentNode(xml, context);

            case "channelselect":
                return new ChannelSelectComponentNode(xml, context);

            case "section":
                return new SectionComponentNode(xml, context);

            case "text" or "p":
                return new TextDisplayComponentNode(xml, context);

            case "thumbnail":
                return new ThumbnailComponentNode(xml, context);

            case "mediagallery" or "gallery":
                return new MediaGalleryComponentNode(xml, context);

            case "file":
                return new FileComponentNode(xml, context);

            case "separator" or "br":
                return new SeparatorComponentNode(xml, context);

            case "container":
                return new ContainerComponentNode(xml, context);

            case "select" or "selection":
                var type = ((CXmlValue.Scalar?) xml.GetAttribute("type")?.Value)?.Value;

                if (type is "channel") goto case "channelselect";
                if (type is "user") goto case "userselect";
                if (type is "role") goto case "roleselect";
                if (type is "mentionable" or "mention") goto case "mentionableselect";
                if (type is "string" or "str") goto case "stringselect";

                goto default;

            default:
                if (TryBindCustomNode() is { } customNode) return customNode;

                context.ReportDiagnostic(
                    Diagnostics.UnknownComponentType,
                    context.GetLocation(xml),
                    xml.Name.Value
                );
                return null;
        }

        ComponentNode? TryBindCustomNode()
        {
            // TODO: Disabled, for now
            return null;

            var symbol = context
                .LookupNode(xml.Name.Value)
                .OfType<ITypeSymbol>()
                .FirstOrDefault(IsValidUserNode);

            if (symbol is null) return null;

            return new CustomComponent(xml, symbol, context);
        }

        bool IsValidUserNode(ITypeSymbol symbol)
            => symbol.TypeKind is TypeKind.Class or TypeKind.Struct &&
               symbol.AllInterfaces.Any(x =>
                   context.KnownTypes.ICXElementType!.Equals(x, SymbolEqualityComparer.Default)
               );
    }

    public virtual void ReportValidationErrors()
    {
        if (Element is not null)
        {
            foreach (var extraAttribute in Element.Attributes.Keys.Except(_consumedProperties))
            {
                Context.ReportDiagnostic(
                    Diagnostics.UnknownAttributeWarning,
                    Context.GetLocation(Element.GetAttribute(extraAttribute)!),
                    extraAttribute,
                    FriendlyName
                );
            }
        }


        foreach (var property in _properties)
        {
            property.Validate(Context);
        }
    }

    public abstract string Render();

    protected ComponentProperty<string> MapProperty(
        string name,
        bool optional = false,
        ValueParseDelegate<string>? parser = null,
        IReadOnlyList<ComponentPropertyValidator<string>>? validators = null,
        Optional<string> defaultValue = default,
        ITypeSymbol? apiType = null,
        params IReadOnlyList<string> aliases
    ) => MapProperty<string>(
        name,
        parser ?? ValueParsers.ParseStringProperty,
        optional,
        validators,
        defaultValue,
        apiType,
        aliases
    );

    protected ComponentProperty<T> MapProperty<T>(
        string name,
        ValueParseDelegate<T> parser,
        bool optional = false,
        IReadOnlyList<ComponentPropertyValidator<T>>? validators = null,
        Optional<T> defaultValue = default,
        ITypeSymbol? apiType = null,
        params IReadOnlyList<string> aliases
    )
    {
        var property = new ComponentProperty<T>(
            this,
            name,
            GetAttribute(name, aliases),
            aliases,
            optional,
            validators ?? [],
            parser,
            defaultValue,
            apiType
        );

        _properties.Add(property);

        return property;
    }

    protected CXmlAttribute? GetAttribute(string name, params IEnumerable<string> aliases)
    {
        if (Element is null) return null;

        CXmlAttribute? attribute = null;

        foreach (var term in aliases.Prepend(name))
        {
            if (Element.GetAttribute(term) is not { } result) continue;

            if (attribute is not null)
            {
                Context.ReportDiagnostic(
                    Diagnostics.DuplicateAttribute,
                    Context.GetLocation(result),
                    result.Name,
                    attribute.Name
                );
                continue;
            }

            attribute = result;
        }

        if (attribute is not null) _consumedProperties.Add(attribute.Name.Value);

        return attribute;
    }
}
