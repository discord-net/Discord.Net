using Discord.Models;
using Discord.Net;
using Discord.Net.Queue;
using Discord.Net.Rest;
using Discord.Rest.Converters;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Discord.Rest;

public partial class DiscordRestApiClient : IRestApiProvider
{
    private static readonly ConcurrentDictionary<string, Func<BucketIds, BucketId>> _bucketIdGenerators = new ();

    private JsonSerializerOptions _serializerOptions;
    protected readonly SemaphoreSlim _stateLock;
    private readonly RestClientProvider _restClientProvider;
    private CancellationTokenSource? _loginCancelToken = null;

    protected bool IsDisposed;

    public RetryMode DefaultRetryMode { get; }
    public string UserAgent { get; }
    public LoginState LoginState { get; private set; }
    public TokenType AuthTokenType { get; private set; }

    internal RequestQueue RequestQueue { get; }
    internal string? AuthToken { get; private set; }
    internal IRestClient RestClient { get; private set; }
    internal ulong? CurrentUserId { get; set; }
    internal ulong? CurrentApplicationId { get; set; }
    internal bool UseSystemClock { get; set; }
    internal Func<IRateLimitInfo, Task>? DefaultRatelimitCallback { get; set; }

    private readonly AsyncEvent<Func<string, string, double, Task>> _sentRequestEvent = new ();


    public DiscordRestApiClient(RestClientProvider restClientProvider, string userAgent, RetryMode defaultRetryMode = RetryMode.AlwaysRetry,
        JsonSerializerOptions? serializer = null, bool useSystemClock = true, Func<IRateLimitInfo, Task>? defaultRatelimitCallback = null)
    {
        _restClientProvider = restClientProvider;
        UserAgent = userAgent;
        DefaultRetryMode = defaultRetryMode;
        UseSystemClock = useSystemClock;
        DefaultRatelimitCallback = defaultRatelimitCallback;

        RequestQueue = new RequestQueue();
        _stateLock = new SemaphoreSlim(1, 1);

        _serializerOptions = new JsonSerializerOptions
        {
            Converters = {
                new OptionalConverter(),
                new UInt64Converter(),
                new EmbedTypeConverter(),
                new UserStatusConverter()
            },
        };

        RestClient = _restClientProvider(DiscordConfig.APIUrl);

        SetBaseUrl(DiscordConfig.APIUrl, false);
    }


    public ValueTask DisposeAsync() => throw new NotImplementedException();

    public void Dispose() => throw new NotImplementedException();

    public Task LoginAsync(TokenType tokenType, string token, bool validateToken, CancellationToken? cancellationToken = null)
        => throw new NotImplementedException();

    public Task LogoutAsync(CancellationToken? cancellationToken = null) => throw new NotImplementedException();

    #region Voice

    public virtual Task<IReadOnlyCollection<IVoiceRegion>> ListVoiceRegionsAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null)
        => throw new NotImplementedException();

    #endregion

    #region Invite

    public virtual Task<IInviteModel> GetInviteAsync(string inviteCode, bool withCounts = true, bool withExpiration = true, ulong? scheduledEventId = null,
        CancellationToken? cancellationToken = null, RequestOptions? options = null)
        => throw new NotImplementedException();

    public virtual Task<IInviteModel> DeleteInviteAsync(string inviteCode, CancellationToken? cancellationToken = null, RequestOptions? options = null)
        => throw new NotImplementedException();

    #endregion

    /// <exception cref="ArgumentException">Unknown OAuth token type.</exception>
    internal void SetBaseUrl(string baseUrl, bool dispose = true)
    {
        if (dispose)
            RestClient?.Dispose();

        RestClient = _restClientProvider(baseUrl);
        RestClient.SetHeader("accept", "*/*");
        RestClient.SetHeader("user-agent", UserAgent);
    }

    /// <exception cref="ArgumentException">Unknown OAuth token type.</exception>
    internal static string GetPrefixedToken(TokenType tokenType, string token)
    {
        return tokenType switch
        {
            TokenType.Bot => $"Bot {token}",
            TokenType.Bearer => $"Bearer {token}",
            _ => throw new ArgumentException(message: "Unknown OAuth token type.", paramName: nameof(tokenType)),
        };
    }

    internal virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                _loginCancelToken?.Dispose();
                RestClient.Dispose();
                RequestQueue.Dispose();
                _stateLock.Dispose();
            }
            IsDisposed = true;
        }
    }

    internal virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                _loginCancelToken?.Dispose();
                RestClient.Dispose();

                if (!(RequestQueue is null))
                    await RequestQueue.DisposeAsync().ConfigureAwait(false);

                _stateLock.Dispose();
            }
            IsDisposed = true;
        }
    }


    #region Net

    internal Task SendAsync(string method, Expression<Func<string>> endpointExpr, BucketIds ids, ClientBucketType clientBucket = ClientBucketType.Unbucketed,
        CancellationToken? cancellationToken = null, RequestOptions? options = null, [CallerMemberName] string? funcName = null)
        => SendAsync(method, GetEndpoint(endpointExpr), GetBucketId(method, ids, endpointExpr, funcName!), clientBucket, cancellationToken, options);

    public async Task SendAsync(string method, string endpoint, BucketId? bucketId = null, ClientBucketType clientBucket = ClientBucketType.Unbucketed,
        CancellationToken? cancellationToken = null, RequestOptions? options = null)
    {
        options ??= new RequestOptions();
        options.HeaderOnly = true;
        options.BucketId = bucketId;

        var request = new RestRequest(RestClient, method, endpoint, GetCancellationToken(cancellationToken), options);
        await SendInternalAsync(method, endpoint, request).ConfigureAwait(false);
    }

    internal Task SendJsonAsync(string method, Expression<Func<string>> endpointExpr, object? payload, BucketIds ids, ClientBucketType clientBucket = ClientBucketType.Unbucketed,
        CancellationToken? cancellationToken = null, RequestOptions? options = null, [CallerMemberName] string? funcName = null)
        => SendJsonAsync(method, GetEndpoint(endpointExpr), payload, GetBucketId(method, ids, endpointExpr, funcName!), clientBucket, cancellationToken, options);

    public async Task SendJsonAsync(string method, string endpoint, object? payload, BucketId? bucketId = null, ClientBucketType clientBucket = ClientBucketType.Unbucketed,
        CancellationToken? cancellationToken = null, RequestOptions? options = null)
    {
        options ??= new RequestOptions();
        options.HeaderOnly = true;
        options.BucketId = bucketId;

        var json = payload is not null
            ? await SerializeJsonAsync(payload)
            : null;
        var request = new JsonRestRequest(RestClient, method, endpoint, json, GetCancellationToken(cancellationToken), options);
        await SendInternalAsync(method, endpoint, request).ConfigureAwait(false);
    }

    internal Task SendMultipartAsync(string method, Expression<Func<string>> endpointExpr, IReadOnlyDictionary<string, object> multipartArgs, BucketIds ids,
         ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions? options = null, [CallerMemberName] string? funcName = null)
        => SendMultipartAsync(method, GetEndpoint(endpointExpr), multipartArgs, GetBucketId(method, ids, endpointExpr, funcName!), clientBucket, options);

    public async Task SendMultipartAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs,
        BucketId? bucketId = null, ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions? options = null)
    {
        options ??= new RequestOptions();
        options.HeaderOnly = true;
        options.BucketId = bucketId;

        var request = new MultipartRestRequest(RestClient, method, endpoint, multipartArgs, options);
        await SendInternalAsync(method, endpoint, request).ConfigureAwait(false);
    }

    internal Task<TResponse> SendAsync<TResponse>(string method, Expression<Func<string>> endpointExpr, BucketIds ids, ClientBucketType clientBucket = ClientBucketType.Unbucketed,
        CancellationToken? cancellationToken = null,RequestOptions ? options = null, [CallerMemberName] string? funcName = null) where TResponse : class
        => SendAsync<TResponse>(method, GetEndpoint(endpointExpr), GetBucketId(method, ids, endpointExpr, funcName!), clientBucket, cancellationToken, options);

    public async Task<TResponse> SendAsync<TResponse>(string method, string endpoint, BucketId? bucketId = null,
        ClientBucketType clientBucket = ClientBucketType.Unbucketed, CancellationToken? cancellationToken = null, RequestOptions? options = null) where TResponse : class
    {
        options ??= new RequestOptions();
        options.BucketId = bucketId;

        var request = new RestRequest(RestClient, method, endpoint, GetCancellationToken(cancellationToken), options);
        return await DeserializeJsonAsync<TResponse>(await SendInternalAsync(method, endpoint, request).ConfigureAwait(false));
    }

    internal Task<TResponse> SendJsonAsync<TResponse>(string method, Expression<Func<string>> endpointExpr, object payload, BucketIds ids,
         ClientBucketType clientBucket = ClientBucketType.Unbucketed, CancellationToken? cancellationToken = null, RequestOptions? options = null, [CallerMemberName] string? funcName = null) where TResponse : class
        => SendJsonAsync<TResponse>(method, GetEndpoint(endpointExpr), payload, GetBucketId(method, ids, endpointExpr, funcName!), clientBucket, cancellationToken, options);

    public async Task<TResponse> SendJsonAsync<TResponse>(string method, string endpoint, object? payload, BucketId? bucketId = null,
        ClientBucketType clientBucket = ClientBucketType.Unbucketed, CancellationToken? cancellationToken = null, RequestOptions? options = null) where TResponse : class
    {
        options ??= new RequestOptions();
        options.BucketId = bucketId;

        var json = payload is not null
            ? await SerializeJsonAsync(payload)
            : null;

        var request = new JsonRestRequest(RestClient, method, endpoint, json, GetCancellationToken(cancellationToken), options);
        return await DeserializeJsonAsync<TResponse>(await SendInternalAsync(method, endpoint, request).ConfigureAwait(false), cancellationToken);
    }

    internal Task<TResponse> SendMultipartAsync<TResponse>(string method, Expression<Func<string>> endpointExpr, IReadOnlyDictionary<string, object> multipartArgs, BucketIds ids,
         ClientBucketType clientBucket = ClientBucketType.Unbucketed, CancellationToken? cancellationToken = null, RequestOptions? options = null, [CallerMemberName] string? funcName = null)
        => SendMultipartAsync<TResponse>(method, GetEndpoint(endpointExpr), multipartArgs, GetBucketId(method, ids, endpointExpr, funcName!), clientBucket, cancellationToken, options);

    public async Task<TResponse> SendMultipartAsync<TResponse>(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs,
        BucketId? bucketId = null, ClientBucketType clientBucket = ClientBucketType.Unbucketed, CancellationToken? cancellationToken = null, RequestOptions? options = null)
    {
        options ??= new RequestOptions();
        options.BucketId = bucketId;

        var request = new MultipartRestRequest(RestClient, method, endpoint, multipartArgs, options);
        return await DeserializeJsonAsync<TResponse>(await SendInternalAsync(method, endpoint, request).ConfigureAwait(false), cancellationToken);
    }

    private async Task<Stream> SendInternalAsync(string method, string endpoint, RestRequest request)
    {
        if (!request.Options.IgnoreState)
            CheckState();

        request.Options.RetryMode ??= DefaultRetryMode;
        request.Options.UseSystemClock ??= UseSystemClock;
        request.Options.RatelimitCallback ??= DefaultRatelimitCallback;

        var stopwatch = Stopwatch.StartNew();
        var responseStream = await RequestQueue.SendAsync(request).ConfigureAwait(false);
        stopwatch.Stop();

        var milliseconds = ToMilliseconds(stopwatch);
        await _sentRequestEvent.InvokeAsync(method, endpoint, milliseconds).ConfigureAwait(false);

        return responseStream;
    }

    #endregion

    #region Utils

    internal CancellationToken GetCancellationToken(CancellationToken? cancellationToken = null)
        => cancellationToken ?? _loginCancelToken!.Token;

    /// <exception cref="InvalidOperationException">Client is not logged in.</exception>
    protected void CheckState()
    {
        if (LoginState != LoginState.LoggedIn)
            throw new InvalidOperationException("Client is not logged in.");
    }

    protected static double ToMilliseconds(Stopwatch stopwatch) => Math.Round((double)stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);

    protected async Task<string> SerializeJsonAsync(object value, CancellationToken? cancellationToken = null)
    {
        using var memory = new MemoryStream(256);

        await JsonSerializer.SerializeAsync(memory, value, _serializerOptions, GetCancellationToken(cancellationToken));
        memory.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(memory);

        return await reader.ReadToEndAsync();
    }

    protected async Task<T> DeserializeJsonAsync<T>(Stream jsonStream, CancellationToken? cancellationToken = null)
        => (await JsonSerializer.DeserializeAsync<T>(jsonStream, _serializerOptions, GetCancellationToken(cancellationToken)))!;

    protected async Task<T?> NullifyNotFound<T>(Task<T> sendTask) where T : class
    {
        try
        {
            var result = await sendTask.ConfigureAwait(false);

            if (sendTask.Exception != null)
            {
                if (sendTask.Exception.InnerException is HttpException x)
                {
                    if (x.HttpCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                }

                throw sendTask.Exception;
            }
            else
                return result;
        }
        catch (HttpException x) when (x.HttpCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    internal class BucketIds
    {
        public ulong GuildId { get; internal set; }
        public ulong ChannelId { get; internal set; }
        public ulong WebhookId { get; internal set; }
        public string? HttpMethod { get; internal set; }

        internal BucketIds(ulong guildId = 0, ulong channelId = 0, ulong webhookId = 0)
        {
            GuildId = guildId;
            ChannelId = channelId;
            WebhookId = webhookId;
        }

        internal object?[] ToArray()
            => new object?[] { HttpMethod, GuildId, ChannelId, WebhookId };

        internal Dictionary<string, string> ToMajorParametersDictionary()
        {
            var dict = new Dictionary<string, string>();
            if (GuildId != 0)
                dict["GuildId"] = GuildId.ToString();
            if (ChannelId != 0)
                dict["ChannelId"] = ChannelId.ToString();
            if (WebhookId != 0)
                dict["WebhookId"] = WebhookId.ToString();
            return dict;
        }

        internal static int? GetIndex(string name)
        {
            return name switch
            {
                "httpMethod" => 0,
                "guildId" => 1,
                "channelId" => 2,
                "webhookId" => 3,
                _ => null,
            };
        }
    }

    private static string GetEndpoint(Expression<Func<string>> endpointExpr)
    {
        return endpointExpr.Compile()();
    }

    private static BucketId GetBucketId(string httpMethod, BucketIds ids, Expression<Func<string>> endpointExpr, string callingMethod)
    {
        ids.HttpMethod ??= httpMethod;
        return _bucketIdGenerators.GetOrAdd(callingMethod, x => CreateBucketId(endpointExpr))(ids);
    }

    private static Func<BucketIds, BucketId> CreateBucketId(Expression<Func<string>> endpoint)
    {
        try
        {
            //Is this a constant string?
            if (endpoint.Body.NodeType == ExpressionType.Constant)
                return x => BucketId.Create(x.HttpMethod!, ((ConstantExpression)endpoint.Body).Value?.ToString() ?? string.Empty, x.ToMajorParametersDictionary());

            var builder = new StringBuilder();
            var methodCall = endpoint.Body as MethodCallExpression;
            var methodArgs = methodCall?.Arguments.ToArray() ?? Array.Empty<Expression>();
            var format = ((ConstantExpression)methodArgs[0]).Value as string ?? string.Empty;

            //Unpack the array, if one exists (happens with 4+ parameters)
            if (methodArgs.Length > 1 && methodArgs[1].NodeType == ExpressionType.NewArrayInit)
            {
                var arrayExpr = methodArgs[1] as NewArrayExpression;
                var elements = arrayExpr?.Expressions.ToArray() ?? Array.Empty<Expression>();
                Array.Resize(ref methodArgs, elements.Length + 1);
                Array.Copy(elements, 0, methodArgs, 1, elements.Length);
            }

            var endIndex = format.IndexOf('?'); //Don't include params
            if (endIndex == -1)
                endIndex = format.Length;

            var lastIndex = 0;
            while (true)
            {
                var leftIndex = format.IndexOf("{", lastIndex);
                if (leftIndex == -1 || leftIndex > endIndex)
                {
                    builder.Append(format, lastIndex, endIndex - lastIndex);
                    break;
                }
                builder.Append(format, lastIndex, leftIndex - lastIndex);
                var rightIndex = format.IndexOf("}", leftIndex);

                var argId = int.Parse(format.Substring(leftIndex + 1, rightIndex - leftIndex - 1), NumberStyles.None, CultureInfo.InvariantCulture);
                var fieldName = GetFieldName(methodArgs[argId + 1]);

                var mappedId = BucketIds.GetIndex(fieldName);

                if (!mappedId.HasValue && rightIndex != endIndex && format.Length > rightIndex + 1 && format[rightIndex + 1] == '/') //Ignore the next slash
                    rightIndex++;

                if (mappedId.HasValue)
                    builder.Append($"{{{mappedId.Value}}}");

                lastIndex = rightIndex + 1;
            }
            if (builder[builder.Length - 1] == '/')
                builder.Remove(builder.Length - 1, 1);

            format = builder.ToString();

            return x => BucketId.Create(x.HttpMethod!, string.Format(format, x.ToArray()), x.ToMajorParametersDictionary());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to generate the bucket id for this operation.", ex);
        }
    }

    private static string GetFieldName(Expression expr)
    {
        if (expr.NodeType == ExpressionType.Convert)
            expr = ((UnaryExpression)expr).Operand;

        if (expr.NodeType != ExpressionType.MemberAccess)
            throw new InvalidOperationException("Unsupported expression");

        return ((MemberExpression)expr).Member.Name;
    }

    private static string WebhookQuery(bool wait = false, ulong? threadId = null)
    {
        var queries = new List<string>() { };
        if (wait)
            queries.Add("wait=true");
        if (threadId.HasValue)
            queries.Add($"thread_id={threadId}");

        return $"{string.Join("&", queries)}";
    }

    #endregion
}
