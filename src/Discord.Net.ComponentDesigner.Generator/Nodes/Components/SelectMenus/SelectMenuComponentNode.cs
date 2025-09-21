using Discord.CX.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.CX.Nodes.Components.SelectMenus;

public sealed class SelectMenuComponentNode : ComponentNode
{
    public sealed class MissingTypeState : ComponentState;

    public sealed class InvalidTypeState : ComponentState
    {
        public string? Kind { get; init; }
    }

    public sealed class StringSelectState : ComponentState;

    public abstract class SelectStateWithDefaults : ComponentState
    {
        public required IReadOnlyList<SelectMenuDefautValue> Defaults { get; init; }
    }

    public sealed class UserSelectState : SelectStateWithDefaults;

    public sealed class RoleSelectState : SelectStateWithDefaults;

    // TODO: channel types?
    public sealed class ChannelSelectState : SelectStateWithDefaults;

    public sealed class MentionableSelectState : SelectStateWithDefaults;

    public override string Name => "select";

    public ComponentProperty CustomId { get; }
    public ComponentProperty Placeholder { get; }
    public ComponentProperty MinValues { get; }
    public ComponentProperty MaxValues { get; }
    public ComponentProperty Required { get; }
    public ComponentProperty Disabled { get; }

    public override IReadOnlyList<ComponentProperty> Properties { get; }

    public SelectMenuComponentNode()
    {
        Properties =
        [
            ComponentProperty.Id,
            CustomId = new(
                "customId",
                isOptional: false,
                renderer: Renderers.String
            ),
            Placeholder = new(
                "placeholder",
                isOptional: true,
                renderer: Renderers.String
            ),
            MinValues = new(
                "minValues",
                isOptional: true,
                aliases: ["min"],
                renderer: Renderers.Integer
            ),
            MaxValues = new(
                "maxValues",
                isOptional: true,
                aliases: ["max"],
                renderer: Renderers.Integer
            ),
            Required = new(
                "required",
                isOptional: true,
                renderer: Renderers.Boolean
            ),
            Disabled = new(
                "disabled",
                isOptional: true,
                renderer: Renderers.Boolean
            )
        ];
    }

    public override ComponentState? Create(ICXNode source, List<CXNode> children)
    {
        if (source is not CXElement element) return null;

        var typeAttribute = element.Attributes
            .FirstOrDefault(x => x.Identifier.Value.ToLowerInvariant() is "type");

        if (typeAttribute is null) return new MissingTypeState() {Source = source};

        if (typeAttribute.Value is not CXValue.StringLiteral {HasInterpolations: false} typeValue)
            return new InvalidTypeState() {Source = source,};

        var kind = typeValue.Tokens.ToString().ToLowerInvariant();
        switch (kind)
        {
            case "string" or "text":
                children.AddRange(element.Children);
                return new StringSelectState() {Source = source};
            case "user":
                return new UserSelectState() {Source = source, Defaults = ExtractDefaultValues()};
            case "role":
                return new RoleSelectState() {Source = source, Defaults = ExtractDefaultValues()};
            case "channel":
                return new ChannelSelectState() {Source = source, Defaults = ExtractDefaultValues()};
            case "mention" or "mentionable":
                return new MentionableSelectState() {Source = source, Defaults = ExtractDefaultValues()};
            default: return new InvalidTypeState() {Source = source, Kind = kind};
        }

        IReadOnlyList<SelectMenuDefautValue> ExtractDefaultValues()
        {
            var result = new List<SelectMenuDefautValue>();

            foreach (var child in element.Children)
            {
                if (child is not CXElement element)
                {
                    // TODO: diagnostics
                    continue;
                }

                if (!Enum.TryParse<SelectMenuDefaultValueKind>(element.Identifier, true, out var kind))
                {
                    // TODO: diagnostics
                    continue;
                }

                if (element.Children.Count is not 1 || element.Children[0] is not CXValue value)
                {
                    // TODO: diagnostics
                    continue;
                }

                result.Add(new(kind, value));
            }

            return result;
        }
    }

    public override void Validate(ComponentState state, ComponentContext context)
    {
        switch (state)
        {
            case MissingTypeState:
                context.AddDiagnostic(
                    Diagnostics.MissingSelectMenuType,
                    state.Source
                );
                return;
            case InvalidTypeState {Kind: var kind}:
                context.AddDiagnostic(
                    kind is not null ? Diagnostics.SpecifiedInvalidSelectMenuType : Diagnostics.InvalidSelectMenuType,
                    state.Source,
                    kind is not null ? [kind] : null
                );
                return;
        }
    }

    private static string RenderDefaultValue(ComponentContext context, SelectMenuDefautValue defaultValue)
        => $"""
            new {context.KnownTypes.SelectMenuDefaultValueType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                id: {Renderers.Snowflake(context, defaultValue.Value)},
                type: {context.KnownTypes.SelectDefaultValueTypeEnumType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{defaultValue.Kind}
            )
            """;

    public override string Render(ComponentState state, ComponentContext context)
        => $"""
            new {context.KnownTypes.SelectMenuBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}({
                string
                    .Join(
                        ",\n",
                        ((IEnumerable<string>)
                        [
                            (
                                state switch
                                {
                                    UserSelectState => "UserSelect",
                                    RoleSelectState => "RoleSelect",
                                    MentionableSelectState => "MentionableSelect",
                                    ChannelSelectState => "ChannelSelect",
                                    _ => string.Empty
                                }
                            ).Map(x => $"type: {context.KnownTypes.ComponentTypeEnumType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{x}"),
                            state.RenderProperties(this, context),
                            state.RenderChildren(context, x => x.Inner is StringSelectOptionComponentNode)
                                .Map(x =>
                                    $"""
                                     options:
                                     [
                                         {x.WithNewlinePadding(4)}
                                     ]
                                     """
                                ),
                            state is SelectStateWithDefaults {Defaults: var defaults}
                                ? string
                                    .Join(
                                        ",\n",
                                        defaults.Select(x => RenderDefaultValue(context, x))
                                    )
                                    .Map(x =>
                                        $"""
                                         defaultValues:
                                         [
                                             {x.WithNewlinePadding(4)}
                                         ]
                                         """
                                    )
                                : string.Empty
                        ]).Where(x => !string.IsNullOrEmpty(x))
                    )
                    .PrefixIfSome(4)
                    .WithNewlinePadding(4)
                    .WrapIfSome("\n")
            })
            """;
}
