namespace Discord;

public class BasicApiRoute(string name, RequestMethod method, string endpoint, BucketInfo? bucket = null)
    : ApiRoute(name, method, endpoint, bucket)
{
}

public class ApiBodyRoute<TRequestBody>(
    string name,
    RequestMethod method,
    string endpoint,
    TRequestBody body,
    ContentType contentType = ContentType.JsonBody,
    BucketInfo? bucket = null)
    : BasicApiRoute(name, method, endpoint, bucket)
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
    : ApiBodyRoute<TRequestBody>(name, method, endpoint,body, contentType, bucket)
    where TRequestBody : class
    where TResponseBody : class
{

}

public sealed class ApiRoute<TResponse>(string name, RequestMethod method, string endpoint, BucketInfo? bucket = null)
    : BasicApiRoute(name, method, endpoint, bucket)
{
}

public abstract class ApiRoute(string name, RequestMethod method, string endpoint, BucketInfo? bucket = null)
{
    public string Name { get; } = name;
    public RequestMethod Method { get; } = method;
    public string Endpoint { get; } = endpoint;
    public BucketInfo? Bucket { get; } = bucket;

    public override string ToString() => $"{Method.ToString().ToUpper()} {Endpoint} ({Name})";
}
