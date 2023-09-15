
using System.Net;

namespace Discord.Utils;

internal class RouteUtils
{
    public static string GetUrlEncodedQueryParams(params (string, object?)[] args)
    {
        if (args.All(x => x.Item2 is null))
            return string.Empty;

        var paramsString = string.Join("&", args.Where(x => x.Item2 is not null)
            .Select(x => GetUrlEncodedQueryParam(x.Item1, x.Item2!)));

        return $"?{paramsString}";
    }

    public static string GetUrlEncodedQueryParam(string key, object value)
        => $"{key}={WebUtility.UrlEncode(value.ToString())}";
}
