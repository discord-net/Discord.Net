namespace Discord.Rest;

public readonly struct APIRoute
{
    public readonly string Name;
    public readonly RequestMethod Method;
    public readonly string Route;
    public readonly ValueTuple<ScopeType, ulong>? Bucket;

    public APIRoute(string name, RequestMethod method, string route, ValueTuple<ScopeType, ulong>? bucket = null)
    {
        Name = name;
        Method = method;
        Route = route;
        Bucket = bucket;
    }
}

public readonly struct APIBodyRoute<TRequestBody>
{
    public readonly string Name;
    public readonly RequestMethod Method;
    public readonly string Route;
    public readonly ValueTuple<ScopeType, ulong>? Bucket;
    public readonly TRequestBody Body;
    public readonly ContentType ContentType;

    public APIBodyRoute(string name, RequestMethod method, string route, TRequestBody body, ContentType contentType = ContentType.JsonBody, ValueTuple<ScopeType, ulong>? bucket = null)
    {
        Name = name;
        Method = method;
        Route = route;
        Body = body;
        Bucket = bucket;
        ContentType = contentType;
    }
}

public readonly struct APIBodyRoute<TRequestBody, TResponseBody>
{
    public readonly string Name;
    public readonly RequestMethod Method;
    public readonly string Route;
    public readonly ValueTuple<ScopeType, ulong>? Bucket;
    public readonly TRequestBody Body;
    public readonly ContentType ContentType;

    public APIBodyRoute(string name, RequestMethod method, string route, TRequestBody body, ContentType contentType = ContentType.JsonBody, ValueTuple<ScopeType, ulong>? bucket = null)
    {
        Name = name;
        Method = method;
        Route = route;
        Body = body;
        Bucket = bucket;
        ContentType = contentType;
    }
}

public readonly struct APIRoute<TResponse>
{
    public readonly string Name;
    public readonly RequestMethod Method;
    public readonly string Route;
    public readonly ValueTuple<ScopeType, ulong>? Bucket;

    public APIRoute(string name, RequestMethod method, string route, ValueTuple<ScopeType, ulong>? bucket = null)
    {
        Name = name;
        Method = method;
        Route = route;
        Bucket = bucket;
    }
}
