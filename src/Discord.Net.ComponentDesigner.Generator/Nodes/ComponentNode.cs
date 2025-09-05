using Discord.ComponentDesigner.Generator.Parser;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.ComponentDesigner.Generator.Nodes;

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
            Id = MapProperty<int>("id", ParseIntProperty, optional: true);
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
        ParseDelegate<string>? parser = null,
        IReadOnlyList<ComponentPropertyValidator<string>>? validators = null,
        Optional<string> defaultValue = default,
        params IReadOnlyList<string> aliases
    ) => MapProperty<string>(
        name,
        parser ?? ParseStringProperty,
        optional,
        validators,
        defaultValue,
        aliases
    );

    protected ComponentProperty<T> MapProperty<T>(
        string name,
        ParseDelegate<T> parser,
        bool optional = false,
        IReadOnlyList<ComponentPropertyValidator<T>>? validators = null,
        Optional<T> defaultValue = default,
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
            defaultValue
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

    private ComponentPropertyValue<T>? ValidateInterpolationType<T>(
        ComponentProperty<T> property,
        CXmlValue.Interpolation value,
        SpecialType specialType
    ) => ValidateInterpolationType<T>(
        property,
        value,
        (symbol) =>
        {
            if (symbol.SpecialType != specialType)
            {
                Context.ReportDiagnostic(
                    Diagnostics.PropertyMismatch,
                    Context.GetLocation(value),
                    property.Name,
                    nameof(Boolean),
                    symbol.ToDisplayString()
                );
                return false;
            }

            return true;
        }
    );

    private ComponentPropertyValue<T>? ValidateInterpolationType<T>(
        ComponentProperty<T> property,
        CXmlValue.Interpolation value,
        Func<ITypeSymbol, bool> validator
    )
    {
        var interpolationInfo = Context.Interpolations[value.InterpolationIndex];

        if (!validator(interpolationInfo.Type))
            return null;

        return property.CreateValue(in interpolationInfo);
    }

    protected ComponentPropertyValue<int>? ParseIntProperty(ComponentProperty<int> property)
    {
        switch (property.Value)
        {
            case null or CXmlValue.Invalid: return null;

            case CXmlValue.Interpolation interpolation:
                return ValidateInterpolationType(property, interpolation, SpecialType.System_Int32);

            case CXmlValue.Multipart multipart:
                throw new NotImplementedException();
            case CXmlValue.Scalar scalar:
                if (int.TryParse(scalar.Value, out var result))
                    return property.CreateValue(result);

                Context.ReportDiagnostic(
                    Diagnostics.InvalidPropertyValue,
                    Context.GetLocation(scalar),
                    scalar.Value,
                    nameof(Int32)
                );
                return null;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected ComponentPropertyValue<bool>? ParseBooleanProperty(ComponentProperty<bool> property)
    {
        if (property is {IsSpecified: true, Value: null})
            return property.CreateValue(true);

        if (!property.IsSpecified)
            return property.CreateValue(false);

        switch (property.Value)
        {
            case null: return null;

            case CXmlValue.Interpolation interpolation:
                return ValidateInterpolationType(property, interpolation, SpecialType.System_Boolean);

            case CXmlValue.Invalid: return null;

            // multiparts are strings
            case CXmlValue.Multipart multipart:
                Context.ReportDiagnostic(
                    Diagnostics.PropertyMismatch,
                    Context.GetLocation(multipart),
                    property.Name,
                    nameof(Boolean),
                    typeof(string)
                );
                return null;

            case CXmlValue.Scalar scalar:
                var str = scalar.Value.ToLowerInvariant();

                if (str is not "true" and not "false")
                {
                    Context.ReportDiagnostic(
                        Diagnostics.PropertyMismatch,
                        Context.GetLocation(scalar),
                        property.Name,
                        nameof(Boolean),
                        typeof(string)
                    );
                    return null;
                }

                return property.CreateValue(str is "true");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected ComponentPropertyValue<ulong>? ParseSnowflakeProperty(ComponentProperty<ulong> property)
    {
        switch (property.Value)
        {
            case null: return null;

            case CXmlValue.Interpolation interpolation:
                return ValidateInterpolationType(property, interpolation, SpecialType.System_UInt64);
            case CXmlValue.Invalid: return null;
            case CXmlValue.Multipart multipart:
                // TODO: we can only verify the non-interpolated parts
                throw new NotImplementedException();
                break;
            case CXmlValue.Scalar scalar:
                if (ulong.TryParse(scalar.Value, out var snowflake))
                {
                    return property.CreateValue(snowflake);
                }

                Context.ReportDiagnostic(
                    Diagnostics.InvalidSnowflakeIdentifier,
                    Context.GetLocation(scalar),
                    scalar.Value
                );

                return null;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected ComponentPropertyValue<string>? ParseEmojiProperty(ComponentProperty<string> property)
    {
        // TODO
        return ParseStringProperty(property);
    }

    protected ComponentPropertyValue<string>? ParseStringProperty(ComponentProperty<string> property)
    {
        switch (property.Value)
        {
            case CXmlValue.Invalid or null: return null;

            case CXmlValue.Interpolation interpolation:
                // any type automatically gets a .ToString() call, so we don't even have to check this
                return property.CreateValue(interpolation);

            case CXmlValue.Multipart multipart:
                return property.CreateValue(multipart);

            case CXmlValue.Scalar scalar:
                return property.CreateValue(scalar.Value);

            default:
                throw new ArgumentOutOfRangeException(nameof(property.Value));
        }
    }

    protected ComponentPropertyValue<T>? ParseEnumProperty<T>(ComponentProperty<T> property) where T : struct
    {
        switch (property.Value)
        {
            case CXmlValue.Invalid or null: return null;

            case CXmlValue.Interpolation interpolation:
                // TODO: we'll have to validate against the actual api type
                throw new NotImplementedException();

            case CXmlValue.Multipart multipart:
                // TODO: the usecase for this may not be great, but its to figure out later
                throw new NotImplementedException();

            case CXmlValue.Scalar scalar:
            {
                if (Enum.TryParse<T>(scalar.Value, out var result))
                    return property.CreateValue(result);

                Context.ReportDiagnostic(
                    Diagnostics.InvalidEnumProperty,
                    Context.GetLocation(scalar),
                    scalar.Value,
                    property.Name,
                    string.Join(", ", Enum.GetNames(typeof(T)))
                );

                return null;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(property.Value));
        }
    }
}
