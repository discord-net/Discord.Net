using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Discord;

public class ApiInRoute<TRequestBody>(
    string name,
    RequestMethod method,
    string endpoint,
    TRequestBody body,
    ContentType contentType = ContentType.JsonBody,
    BucketInfo? bucket = null
) :
    ApiRoute(name, method, endpoint, bucket),
    IApiInRoute<TRequestBody>
{
    public TRequestBody RequestBody { get; } = body;
    public ContentType ContentType { get; } = contentType;
}

public sealed class ApiInOutRoute<TRequestBody, TResponseBody>(
    string name,
    RequestMethod method,
    string endpoint,
    TRequestBody body,
    ContentType contentType = ContentType.JsonBody,
    BucketInfo? bucket = null
) :
    ApiInRoute<TRequestBody>(name, method, endpoint, body, contentType, bucket),
    IApiInOutRoute<TRequestBody, TResponseBody>
    where TRequestBody : class
    where TResponseBody : class;

public sealed class ApiOutRoute<TResponse>(
    string name,
    RequestMethod method,
    string endpoint,
    BucketInfo? bucket = null
) :
    ApiRoute(name, method, endpoint, bucket),
    IApiOutRoute<TResponse>;

public class ApiRoute(string name, RequestMethod method, string endpoint, BucketInfo? bucket = null) : IApiRoute
{
    public string Name { get; } = name;
    public RequestMethod Method { get; } = method;
    public string Endpoint { get; } = endpoint;
    public BucketInfo? Bucket { get; } = bucket;

    public override string ToString() => $"{Method.ToString().ToUpper()} {Endpoint} ({Name})";
}

public interface IApiInOutRoute<out TParams, out TBody> :
    IApiInRoute<TParams>,
    IApiOutRoute<TBody>;

public interface IApiInRoute<out TParams> : IApiRoute
{
    TParams RequestBody { get; }
    ContentType ContentType { get; }
}

public interface IApiOutRoute<out TBody> : IApiRoute;

public interface IApiRoute
{
    string Name { get; }
    RequestMethod Method { get; }
    string Endpoint { get; }
    BucketInfo? Bucket { get; }
}
