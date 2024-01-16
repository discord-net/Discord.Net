
using System.Net;

namespace Discord.Utils;

internal class RouteUtils
{
    public static string GetUrlEncodedQueryParams(params (string Name, object? Value)[] args)
    {
        if (args.All(x => x.Value is null))
            return string.Empty;

        var paramsString = string.Join("&", args.Where(x => x.Value is not null)
            .Select(x => GetUrlEncodedQueryParam(x.Name, x.Value!)));

        return $"?{paramsString}";
    }

    public static string GetUrlEncodedQueryParams((string Name, object? Value) arg)
        => arg.Value is null
            ? string.Empty
            : $"?{GetUrlEncodedQueryParam(arg.Name, arg.Value!)}";

    private static string GetUrlEncodedQueryParam(string key, object value)
        => $"{key}={WebUtility.UrlEncode(value.ToString())}";
}
