using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Discord.Net.Hanz.Utils.Bakery;

public readonly record struct IndexerSpec(
    string Type,
    ImmutableEquatableArray<ParameterSpec>? Parameters = null,
    Accessibility Accessibility = Accessibility.NotApplicable,
    ImmutableEquatableArray<string>? Modifiers = null,
    string? ExplicitInterfaceImplementation = null,
    Accessibility AutoGet = Accessibility.Public,
    Accessibility AutoSet = Accessibility.NotApplicable,
    string? Getter = null,
    string? Setter = null,
    string? Expression = null
)
{
    public ImmutableEquatableArray<string> Modifiers { get; init; }
        = Modifiers ?? ImmutableEquatableArray<string>.Empty;

    public ImmutableEquatableArray<ParameterSpec> Parameters { get; init; }
        = Parameters ?? ImmutableEquatableArray<ParameterSpec>.Empty;

    public bool HasAutoGetter => AutoGet is not Accessibility.NotApplicable;
    public bool HasAutoSetter => AutoSet is not Accessibility.NotApplicable;

    public bool HasGetter
        => Getter is not null || HasAutoGetter;

    public bool HasSetter
        => Setter is not null || HasAutoSetter;

    public override string ToString()
    {
        var builder = new StringBuilder();

        if (Accessibility is not Accessibility.NotApplicable)
            builder.Append(SyntaxFacts.GetText(Accessibility)).Append(' ');

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

        builder
            .Append("this[")
            .Append(string.Join(", ", Parameters.Select(x => x.ToString())))
            .Append(']');

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
                        .Append(SyntaxFacts.GetText(AutoGet))
                        .Append(' ');

                builder.Append("get; ");

                if (HasAutoSetter)
                {
                    if (AutoSet is not Accessibility.Public)
                        builder
                            .Append(SyntaxFacts.GetText(AutoSet))
                            .Append(' ');

                    builder.Append("set; ");
                }

                builder.Append('}');
            }
            else
            {
                builder.AppendLine()
                    .AppendLine("{")
                    .Append(string.Empty.PrefixSpaces(4));

                if (HasAutoGetter && AutoGet is not Accessibility.Public)
                    builder
                        .Append(SyntaxFacts.GetText(AutoGet))
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
                        .Append(string.Empty.PrefixSpaces(4));

                    if (HasAutoSetter && AutoSet is not Accessibility.Public)
                        builder
                            .Append(SyntaxFacts.GetText(AutoSet))
                            .Append(' ');

                    builder.Append("set");

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