namespace Discord;

public sealed class BasicApiRoute(string name, RequestMethod method, string endpoint, BucketInfo? bucket = null)
    : ApiRoute(name, method, endpoint, bucket)
{}

public sealed class ApiBodyRoute<TRequestBody>(
    string name,
    RequestMethod method,
    string endpoint,
    TRequestBody body,
    ContentType contentType = ContentType.JsonBody,
    BucketInfo? bucket = null)
    : ApiRoute(name, method, endpoint, bucket)
{
    public TRequestBody Body { get; } = body;
    public ContentType ContentType { get; } = contentType;
}

public sealed class ApiBodyRoute<TRequestBody, TResponseBody>(
    string name,
    RequestMethod method,
    string endpoint,
    TRequestBody body,
    ContentType contentType = ContentType.JsonBody,
    BucketInfo? bucket = null)
    : ApiRoute(name, method, endpoint, bucket)
{
    public TRequestBody Body { get; } = body;
    public ContentType ContentType { get; } = contentType;
}

public sealed class ApiRoute<TResponse>(string name, RequestMethod method, string endpoint, BucketInfo? bucket = null)
    : ApiRoute(name, method, endpoint, bucket)
{}

public abstract class ApiRoute(string name, RequestMethod method, string endpoint, BucketInfo? bucket = null)
{
    public string Name { get; } = name;
    public RequestMethod Method { get; } = method;
    public string Endpoint { get; } = endpoint;
    public BucketInfo? Bucket { get; } = bucket;

    public override string ToString()
    {
        return $"{Method.ToString().ToUpper()} {Endpoint} ({Name})";
    }
}
