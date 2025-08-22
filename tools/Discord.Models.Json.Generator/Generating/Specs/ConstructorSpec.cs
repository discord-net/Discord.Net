using System.Text;

namespace Discord.Models.Json.Generator.Specs;


public sealed record ConstructorSpec(
    string Name,
    Accessibility Accessibility = Accessibility.Public,
    IReadOnlyCollection<ParameterSpec>? Parameters = null,
    string? Body = null,
    string? BaseInvocation = null
)
{
    public IReadOnlyCollection<ParameterSpec> Parameters { get; init; } =
        Parameters ?? [];

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append(Accessibility.ToKeywords())
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
                    .Prefix(4)
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