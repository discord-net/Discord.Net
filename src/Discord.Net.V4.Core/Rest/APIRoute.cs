namespace Discord;

public sealed class ApiRoute(string name, RequestMethod method, string route, BucketInfo? bucket = null)
    : IApiRoute
{
    public string Name { get; } = name;
    public RequestMethod Method { get; } = method;
    public string Endpoint { get; } = route;
    public BucketInfo? Bucket { get; } = bucket;
}

public sealed class ApiBodyRoute<TRequestBody>(
    string name,
    RequestMethod method,
    string route,
    TRequestBody body,
    ContentType contentType = ContentType.JsonBody,
    BucketInfo? bucket = null)
    : IApiRoute
{
    public string Name { get; } = name;
    public RequestMethod Method { get; } = method;
    public string Endpoint { get; } = route;
    public BucketInfo? Bucket { get; } = bucket;
    public TRequestBody Body { get; } = body;
    public ContentType ContentType { get; } = contentType;
}

public sealed class ApiBodyRoute<TRequestBody, TResponseBody>(
    string name,
    RequestMethod method,
    string route,
    TRequestBody body,
    ContentType contentType = ContentType.JsonBody,
    BucketInfo? bucket = null)
    : IApiRoute
{
    public string Name { get; } = name;
    public RequestMethod Method { get; } = method;
    public string Endpoint { get; } = route;
    public BucketInfo? Bucket { get; } = bucket;
    public TRequestBody Body { get; } = body;
    public ContentType ContentType { get; } = contentType;
}

public sealed class ApiRoute<TResponse>(string name, RequestMethod method, string route, BucketInfo? bucket = null)
    : IApiRoute
{
    public string Name { get; } = name;
    public RequestMethod Method { get; } = method;
    public string Endpoint { get; } = route;
    public BucketInfo? Bucket { get; } = bucket;
}

public interface IApiRoute
{
    string Name { get; }
    RequestMethod Method { get; }
    string Endpoint { get; }
    BucketInfo? Bucket { get; }
}
