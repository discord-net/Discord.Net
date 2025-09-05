using Microsoft.CodeAnalysis;

namespace Discord.ComponentDesigner.Generator;

public static class Diagnostics
{
    public static readonly DiagnosticDescriptor UnknownComponentType = new(
        "DC0001",
        "Unknown component type",
        "Unknown component '{0}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor EmptyActionRow = new(
        "DC0002",
        "Action row empty",
        "An action row must contain at least one child",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TooManyChildrenInActionRow = new(
        "DC0003",
        "Too many children in action row",
        "An action row can contain up to 5 buttons OR 1 select menu",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor ActionRowCanOnlyContainMultipleButtons = new(
        "DC0004",
        "Invalid mix of components in action row",
        "'{0}' is not a valid child of this action row, an action row can ONLY contain up to 5 buttons OR 1 select menu",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor ButtonLabelMaxLengthExceeded = new(
        "DC0005",
        "Button label too long",
        "A buttons label may only be at most 80 characters long",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidSnowflakeIdentifier = new(
        "DC0006",
        "Invalid snowflake identifier",
        "'{0}' is not a valid snowflake identifier",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor UrlAndCustomIdBothSpecified = new(
        "DC0007",
        "Invalid button configuration",
        "A button may not contain a URL and a custom id",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingButtonUrl = new(
        "DC0008",
        "Missing 'url' attribute for link-style button",
        "A 'Link' button must contain a URL attribute",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor CustomIdTooLong = new(
        "DC0009",
        "Custom Id too long",
        "A custom id may only be a maximum of 80 characters long",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidContainerChild = new(
        "DC0010",
        "Invalid Container Child",
        "The component '{0}' may not be used as a child of a container, valid components are: Action Rows, Text Displays, Media Galleries, Separators, and Files",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingCustomId = new(
        "DC0011",
        "Missing custom ID",
        "The '{0}' component requires a custom ID",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor PlaceholderTooLong = new(
        "DC0012",
        "Placeholder too long",
        "A placeholder may only be a maximum of {0} characters long",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MinValuesTooSmall = new(
        "DC0013",
        "Invalid minimum value",
        "The minimum number of items must be at least '{0}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );
    public static readonly DiagnosticDescriptor MinValuesTooLarge = new(
        "DC0014",
        "Invalid minimum value",
        "The minimum number of items must be at most '{0}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MaxValuesTooLarge = new(
        "DC0015",
        "Invalid maximum value",
        "The maximum number of items must be at most '{0}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );
    public static readonly DiagnosticDescriptor MaxValuesTooSmall = new(
        "DC0016",
        "Invalid maximum value",
        "The maximum number of items must be at least '{0}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor EmptyMediaGallery = new(
        "DC0017",
        "A media gallery cannot be empty",
        $"A media gallery must contain at least one media item and at most {Constants.MAX_MEDIA_ITEMS}",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TooManyMediaGalleryItems = new(
        "DC0018",
        "Too many media gallery items",
        $"A media gallery must contain at most {Constants.MAX_MEDIA_ITEMS} media items",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MediaGalleryItemDescriptionTooLong = new(
        "DC0019",
        "Media gallery item description length exceeded",
        $"A media gallery items' description must contain at most {Constants.MAX_MEDIA_ITEM_DESCRIPTION_LENGTH} characters",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidSectionChildNode = new(
        "DC0020",
        "Invalid section child",
        $"A section may only contain Text Display components",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingSectionComponents = new(
        "DC0021",
        "Missing section child component",
        $"A section must contain at least one child that is not an accessory",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TooManySectionComponentChildren = new(
        "DC0022",
        "Too many section component children",
        $"A section must contain at most {Constants.MAX_SECTION_CHILDREN} non-accessory components",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingAccessory = new(
        "DC0023",
        "Missing accessory",
        $"A section must contain an accessory",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor ExtraAccessory = new(
        "DC0024",
        "Extra accessory",
        $"A section can only contain at most 1 accessory",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor StringSelectOptionLabelTooLong = new(
        "DC0025",
        "Label too long",
        $"A string selection options' label must be at most {Constants.STRING_SELECT_OPTION_LABEL_MAX_LENGTH} characters",
        "Components",
        DiagnosticSeverity.Error,
        true
    );
    public static readonly DiagnosticDescriptor StringSelectOptionValueTooLong = new(
        "DC0026",
        "Label too long",
        $"A string selection options' value must be at most {Constants.STRING_SELECT_OPTION_VALUE_MAX_LENGTH} characters",
        "Components",
        DiagnosticSeverity.Error,
        true
    );
    public static readonly DiagnosticDescriptor StringSelectOptionDescriptionTooLong = new(
        "DC0027",
        "Label too long",
        $"A string selection options' description must be at most {Constants.MAX_MEDIA_ITEM_DESCRIPTION_LENGTH} characters",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingTextInputLabel = new(
        "DC0028",
        "Missing label",
        "Text input requires a 'label' attribute",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TextInputLabelTooLong = new(
        "DC0029",
        "Label too long",
        $"A text inputs' label must be at most {Constants.TEXT_INPUT_LABEL_MAX_LENGTH} characters",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TextInputMinValueOutOfRange = new(
        "DC0030",
        "Min value out of range",
        $"A text inputs' min value must be between {Constants.TEXT_INPUT_MIN_LENGTH_MIN_VALUE} and {Constants.TEXT_INPUT_MIN_LENGTH_MAX_VALUE}",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TextInputMaxValueOutOfRange = new(
        "DC0031",
        "Max value out of range",
        $"A text inputs' max value must be between {Constants.TEXT_INPUT_MIN_LENGTH_MIN_VALUE} and {Constants.TEXT_INPUT_MIN_LENGTH_MAX_VALUE}",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TextInputValueTooLong = new(
        "DC0032",
        "Value is too long",
        $"A text inputs' value must be at most {Constants.TEXT_INPUT_VALUE_MAX_LENGTH} characters",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TextInputPlaceholderTooLong = new(
        "DC0033",
        "Placeholder is too long",
        $"A text inputs' placeholder must be at most {Constants.TEXT_INPUT_PLACEHOLDER_MAX_LENGTH} characters",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor ThumbnailDescriptionTooLong = new(
        "DC0034",
        "Description is too long",
        $"A thumbnails' description must be at most {Constants.THUMBNAIL_DESCRIPTION_MAX_LENGTH} characters",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingThumbnailUrl = new(
        "DC0035",
        "Missing URL",
        $"A thumbnail must contain a url",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingButtonCustumIdOrUrl = new(
        "DC0036",
        "Missing custom id or URL",
        $"A button must contain either a custom id or URL",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidEnumProperty = new(
        "DC0037",
        "Invalid attribute value",
        "'{0}' is not reconized as a valid value of '{1}', accepted values are: {3}",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor PropertyMismatch = new(
        "DC0038",
        "Invalid type for attribute",
        "'{0}' expects a value of type '{1}', but found '{2}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor DuplicateAttribute = new(
        "DC0039",
        "Duplicate attribute specification",
        "'{0}' refers to the already provided attribute '{1}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingRequiredProperty = new(
        "DC0040",
        "Missing required attribute",
        "'{0}' requires the attribute '{1}' to be specified",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor StringTooShort = new(
        "DC0041",
        "String value is too short",
        "'{0}' must be at least {1} characters long",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor StringTooLong = new(
        "DC0042",
        "String value is too long",
        "'{0}' must be at most {1} characters long",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidPropertyValue = new(
        "DC0043",
        "Invalid attribute value",
        "'{0}' is not reconized as a valid value of '{1}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidChildNodeType = new(
        "DC0044",
        "Invalid child",
        "'{0}' cannot contain children of type '{1}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidChildComponentType = new(
        "DC0045",
        "Invalid child component",
        "'{0}' cannot contain children of type '{1}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TooManyChildren = new(
        "DC0046",
        "Too Many Children",
        "'{0}' can only contain up to {1} children",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TextCannotContainComponents = new(
        "DC0047",
        "Invalid child",
        "Text displays cannot contain any components",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor ComponentParseError = new(
        "DC0048",
        "Invalid component markup",
        "{0}",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor UnknownAttributeWarning = new(
        "DC0049",
        "Unknown attribute",
        "'{0}' is not reconized as an attribute of '{1}'",
        "Components",
        DiagnosticSeverity.Warning,
        true
    );

    public static readonly DiagnosticDescriptor ButtonChildLabelError = new(
        "DC0050",
        "Invalid child for button",
        "A button can only contain one text element as a child",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor ButtonDuplicateLabels = new(
        "DC0051",
        "Duplicate button label",
        "A buttons' label can only be specified once!",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor PossibleInvalidChildNodeType = new(
        "DC0052",
        "Possible invalid child node type",
        "'{0}' may not be a valid child of '{1}'",
        "Components",
        DiagnosticSeverity.Warning,
        true
    );

    public static readonly DiagnosticDescriptor InvalidAttributeType = new(
        "DC0053",
        "Invalid attribute value type",
        "'{0}' is not assignable to '{1}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingLabelChildren = new(
        "DC0054",
        "Missing label text and component",
        "Labels require some text and a component",
        "Components",
        DiagnosticSeverity.Error,
        true
    );
}
