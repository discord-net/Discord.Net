using System.Text;

namespace Discord.Models.Json.Generator.Specs;

public sealed record PropertySpec(
    string Type,
    string Name,
    Accessibility Accessibility = Accessibility.NotApplicable,
    IReadOnlyCollection<string>? Modifiers = null,
    string? ExplicitInterfaceImplementation = null,
    Accessibility AutoGet = Accessibility.Public,
    Accessibility AutoSet = Accessibility.NotApplicable,
    string? Getter = null,
    string? Setter = null,
    string? Expression = null,
    string? EqualsClause = null,
    bool Init = false,
    IReadOnlyCollection<AttributeSpec>? Attributes = null)
{
    public IReadOnlyCollection<string> Modifiers { get; init; }
        = Modifiers ?? [];

    public IReadOnlyCollection<AttributeSpec> Attributes { get; init; } =
        Attributes ?? [];

    public bool HasAutoGetter => AutoGet is not Accessibility.NotApplicable;
    public bool HasAutoSetter => AutoSet is not Accessibility.NotApplicable;

    public bool HasGetter
        => Getter is not null || HasAutoGetter;

    public bool HasSetter
        => Setter is not null || HasAutoSetter;

    private string SetterKeyword => Init ? "init" : "set";

    public override string ToString()
    {
        var builder = new StringBuilder();

        if (Attributes.Count > 0)
        {
            builder.Append('[');
            foreach (var attribute in Attributes)
            {
                builder.Append(attribute).Append(", ");
            }

            builder.Length -= 2;
            builder.AppendLine("]");
        }
        
        if (Accessibility is not Accessibility.NotApplicable)
            builder.Append(Accessibility.ToKeywords()).Append(' ');

        if (Modifiers.Count > 0)
        {
            builder
                .Append(string.Join(" ", Modifiers))
                .Append(' ');
        }

        builder.Append(Type).Append(' ');

        if (ExplicitInterfaceImplementation is not null)
            builder
                .Append(ExplicitInterfaceImplementation)
                .Append('.');

        builder.Append(Name);

        if (Expression is not null)
        {
            builder
                .Append(" => ")
                .Append(Expression)
                .Append(';');
        }
        else if (HasGetter || HasSetter)
        {
            if (Getter is null && Setter is null)
            {
                builder.Append(" { ");

                if (AutoGet is not Accessibility.Public)
                    builder
                        .Append(AutoGet.ToKeywords())
                        .Append(' ');

                builder.Append("get; ");

                if (HasAutoSetter)
                {
                    if (AutoSet is not Accessibility.Public)
                        builder
                            .Append(AutoSet.ToKeywords())
                            .Append(' ');

                    builder.Append(SetterKeyword).Append("; ");
                }

                builder.Append('}');

                if (EqualsClause is not null)
                    builder.Append($" = {EqualsClause.WithNewlinePadding(4)};");
            }
            else
            {
                builder.AppendLine()
                    .AppendLine("{")
                    .Append(string.Empty.Prefix(4));

                if (HasAutoGetter && AutoGet is not Accessibility.Public)
                    builder
                        .Append(AutoGet.ToKeywords())
                        .Append(' ');

                builder.Append("get");

                if (Getter is not null)
                {
                    builder.Append(Getter.WithNewlinePadding(4));
                }
                else
                {
                    builder.Append(';');
                }

                if (HasSetter)
                {
                    builder
                        .AppendLine()
                        .Append(string.Empty.Prefix(4));

                    if (HasAutoSetter && AutoSet is not Accessibility.Public)
                        builder
                            .Append(AutoSet.ToKeywords())
                            .Append(' ');

                    builder.Append(SetterKeyword);

                    if (Setter is not null)
                    {
                        builder.Append(Setter.WithNewlinePadding(4));
                    }
                    else
                    {
                        builder.Append(';');
                    }
                }

                builder
                    .AppendLine()
                    .Append('}');
            }
        }

        return builder.ToString();
    }
}