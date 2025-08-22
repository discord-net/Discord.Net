using System.Text;

namespace Discord.Models.Json.Generator.Specs;


public sealed record ParameterSpec(
    string Type,
    string Name,
    string? Default = null
)
{
    public string ToString(bool includeDefault)
    {
        var builder = new StringBuilder()
            .Append(Type)
            .Append(' ')
            .Append(Name);

        if (includeDefault && Default is not null)
            builder.Append(" = ").Append(Default);

        return builder.ToString();
    }

    public override string ToString() => ToString(true);

    public static implicit operator ParameterSpec((string, string) tuple) => new(tuple.Item1, tuple.Item2);

    public static implicit operator ParameterSpec((string, string, string?) tuple) =>
        new(tuple.Item1, tuple.Item2, tuple.Item3);
}