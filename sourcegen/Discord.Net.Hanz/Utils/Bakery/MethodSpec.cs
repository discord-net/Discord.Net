using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Discord.Net.Hanz.Utils.Bakery;

public readonly record struct MethodSpec(
    string Name,
    string ReturnType,
    Accessibility Accessibility = Accessibility.NotApplicable,
    ImmutableEquatableArray<string>? Modifiers = null,
    ImmutableEquatableArray<ParameterSpec>? Parameters = null,
    ImmutableEquatableArray<GenericSpec>? Generics = null,
    ImmutableEquatableArray<GenericConstraintSpec>? GenericConstraints = null,
    string? ExplicitInterfaceImplementation = null,
    string? Expression = null,
    string? Body = null
)
{
    public ImmutableEquatableArray<string> Modifiers { get; init; }
        = Modifiers ?? ImmutableEquatableArray<string>.Empty;

    public ImmutableEquatableArray<ParameterSpec> Parameters { get; init; }
        = Parameters ?? ImmutableEquatableArray<ParameterSpec>.Empty;

    public ImmutableEquatableArray<GenericSpec> Generics { get; init; }
        = Generics ?? ImmutableEquatableArray<GenericSpec>.Empty;

    public ImmutableEquatableArray<GenericConstraintSpec> GenericConstraints { get; init; }
        = GenericConstraints ?? ImmutableEquatableArray<GenericConstraintSpec>.Empty;

    public override string ToString()
    {
        var builder = new StringBuilder();

        if (ExplicitInterfaceImplementation is null && Accessibility is not Accessibility.NotApplicable)
        {
            builder
                .Append(SyntaxFacts.GetText(Accessibility))
                .Append(' ');
        }

        if (Modifiers.Count > 0)
        {
            builder.Append(string.Join(" ", Modifiers)).Append(' ');
        }

        builder.Append(ReturnType).Append(' ');

        if (ExplicitInterfaceImplementation is not null)
            builder.Append(ExplicitInterfaceImplementation).Append('.');

        builder.Append(Name);

        if (Generics.Count > 0)
        {
            builder
                .Append('<')
                .Append(string.Join(", ", Generics))
                .Append('>');
        }

        builder.Append('(');

        if (Parameters.Count > 2)
        {
            var includeParameterDefaults = ExplicitInterfaceImplementation is null;
            
            builder.AppendLine()
                .AppendLine(string
                    .Join(
                        $",{Environment.NewLine}",
                        Parameters.Select(x => x.ToString(includeParameterDefaults))
                    )
                    .PrefixSpaces(4)
                    .WithNewlinePadding(4)
                );
        }
        else if (Parameters.Count > 0)
            builder.Append(string.Join(", ", Parameters));

        builder
            .Append(')');

        if (GenericConstraints.Count > 0)
        {
            builder
                .AppendLine()
                .Append(string.Join(Environment.NewLine, GenericConstraints).PrefixSpaces(4));
        }

        if (Expression is not null)
        {
            builder.Append($" => {Expression};");
        }
        else if (Body is not null)
        {
            builder.AppendLine()
                .AppendLine("{")
                .AppendLine()
                .AppendLine(Body)
                .AppendLine("}");
        }
        else
        {
            builder.Append(";");
        }

        return builder.ToString();
    }
}