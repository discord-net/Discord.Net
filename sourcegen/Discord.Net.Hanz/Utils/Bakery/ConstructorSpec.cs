using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Discord.Net.Hanz.Utils.Bakery;

public readonly record struct ConstructorSpec(
    string Name,
    Accessibility Accessibility
)
{
    public ImmutableEquatableArray<ParameterSpec> Parameters { get; init; } =
        ImmutableEquatableArray<ParameterSpec>.Empty;

    public string? BaseInvocation { get; init; }
    public string? Body { get; init; }

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append(SyntaxFacts.GetText(Accessibility))
            .Append(' ')
            .Append(Name)
            .Append('(');

        if (Parameters.Count > 0)
        {
            builder.AppendLine()
                .AppendLine(string
                    .Join(
                        $",{Environment.NewLine}",
                        Parameters
                    )
                    .PrefixSpaces(4)
                    .WithNewlinePadding(4)
                );
        }

        builder
            .Append(')');

        if (BaseInvocation is not null)
            builder.Append(" : ").AppendLine(BaseInvocation);

        builder.AppendLine("{");

        if (Body is not null)
            builder.AppendLine(Body.WithNewlinePadding(4));

        return builder.Append("}").ToString();
    }
}