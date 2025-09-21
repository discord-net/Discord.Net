using Microsoft.CodeAnalysis;

namespace Discord.CX;

public static partial class Diagnostics
{
    public static readonly DiagnosticDescriptor ParseError = new(
        "DCP001",
        "CX Parsing error",
        "{0}",
        "Component Parser (CX)",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidEnumVariant = new(
        "DC0001",
        "Invalid enum variant",
        "'{0}' is not a valid variant of '{1}'; valid values are '{2}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TypeMismatch = new(
        "DC0002",
        "Type mismatch",
        "'{0}' is not of expected type '{1}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor OutOfRange = new(
        "DC0003",
        "Type mismatch",
        "'{0}' must be {1} in length",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor UnknownComponent = new(
        "DC0004",
        "Unknown component",
        "'{0}' is not a known component",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor ButtonCustomIdUrlConflict = new(
        "DC0005",
        "Invalid button",
        "Buttons cannot contain both a 'url' and a 'customid'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor ButtonCustomIdOrUrlMissing = new(
        "DC0006",
        "Invalid button",
        "A button must specify either a 'customId' or a 'url'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor LinkButtonUrlMissing = new(
        "DC0007",
        "Invalid button",
        "A 'link' button must specify 'url'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor PremiumButtonSkuMissing = new(
        "DC0008",
        "Invalid button",
        "A 'premium' button must specify 'skuId'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor PremiumButtonPropertyNotAllowed = new(
        "DC0009",
        "Invalid button",
        "A 'premium' button cannot specify '{0}'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor ButtonLabelDuplicate = new(
        "DC0010",
        "Duplicate label definition",
        "A button cannot specify both a body and a 'label'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor EmptyActionRow = new(
        "DC0011",
        "Empty Action Row",
        "An action row must contain at least one child",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingRequiredProperty = new(
        "DC0012",
        "Missing Property",
        "'{0}' requires the property '{1}' to be specified",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor UnknownProperty = new(
        "DC0013",
        "Unknown Property",
        "'{0}' is not a known property of '{1}'",
        "Components",
        DiagnosticSeverity.Warning,
        true
    );

    public static readonly DiagnosticDescriptor EmptyAccessory = new(
        "DC0014",
        "Empty Accessory",
        "An accessory must have 1 child",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TooManyAccessoryChildren = new(
        "DC0015",
        "Too many accessory children",
        "An accessory must have 1 child",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor EmptySection = new(
        "DC0016",
        "Section cannot be empty",
        "A section must have an accessory and a child",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidAccessoryChild = new(
        "DC0017",
        "Invalid accessory child",
        "'{0}' is not a valid accessory, only buttons and thumbnails are allowed",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingAccessory = new(
        "DC0018",
        "Missing accessory",
        "A section must contain an accessory",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TooManyAccessories = new(
        "DC0019",
        "Too many accessories",
        "A section can only contain one accessory",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingSectionChild = new(
        "DC0020",
        "Missing section child",
        "A section must contain at least 1 non-accessory component",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor TooManySectionChildren = new(
        "DC0021",
        "Too many section children",
        "A section must contain at most 3 non-accessory components",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidSectionChildComponentType = new(
        "DC0022",
        "Invalid section child component type",
        "'{0}' is not a valid child component of a section; only text displays are allowed",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingSelectMenuType = new(
        "DC0023",
        "Missing select menu type",
        "You must specify the type of the select menu, being one of 'string', 'user', 'role', 'channel', or 'mentionable'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidSelectMenuType = new(
        "DC0024",
        "Invalid select menu type",
        "Select menu type must be either 'string', 'user', 'role', 'channel', or 'mentionable'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor SpecifiedInvalidSelectMenuType = new(
        "DC0025",
        "Invalid select menu type",
        "'{0}' is not a valid elect menu type; must be either 'string', 'user', 'role', 'channel', or 'mentionable'",
        "Components",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor ActionRowInvalidChild = new(
        "DC0026",
        "Invalid action row child component",
        "An action row can only contain 1 select menu OR at most 5 buttons",
        "Components",
        DiagnosticSeverity.Error,
        true
    );
}
