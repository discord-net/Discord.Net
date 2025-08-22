using System.Text;

namespace Discord.Models;

public class QueryStrings(IEnumerable<KeyValuePair<string, object>> values) : Dictionary<string, object>(values)
{
    public static string Build(
       string name,
       object? value
    )
    {
        if (value is null) return string.Empty;

        return $"?{name}={value}";
    }
    public static string Build(
        params (string, object?)[] values
    )
    {
        if (values.Length is 0) return string.Empty;

        return
            new QueryStrings(
                values
                    .Where(x => x.Item2 is not null)
                    .Select(x => new KeyValuePair<string, object>(x.Item1, x.Item2!))
            )
            .ToString();
    }

    public override string ToString()
    {
        if (Count is 0) return string.Empty;

        var sb = new StringBuilder("?");

        foreach (var pair in this)
        {
            if (sb.Length is not 1) sb.Append('&');

            sb.Append(pair.Key).Append('=').Append(pair.Value);
        }

        return sb.ToString();
    }
}