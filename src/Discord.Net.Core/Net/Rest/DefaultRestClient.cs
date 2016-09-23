using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    public sealed class DefaultRestClient : IRestClient
    {
        private const int HR_SECURECHANNELFAILED = -2146233079;

        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly JsonSerializer _errorDeserializer;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken, _parentToken;
        private bool _isDisposed;

        public DefaultRestClient(string baseUrl)
        {
            _baseUrl = baseUrl;

            _client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = false,
                UseProxy = false
            });
            SetHeader("accept-encoding", "gzip, deflate");

            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;
            _errorDeserializer = new JsonSerializer();
        }
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                    _client.Dispose();
                _isDisposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        public void SetHeader(string key, string value)
        {
            _client.DefaultRequestHeaders.Remove(key);
            if (value != null)
                _client.DefaultRequestHeaders.Add(key, value);
        }
        public void SetCancelToken(CancellationToken cancelToken)
        {
            _parentToken = cancelToken;
            _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _cancelTokenSource.Token).Token;
        }

        public async Task<Stream> SendAsync(string method, string endpoint, bool headerOnly = false)
        {
            string uri = Path.Combine(_baseUrl, endpoint);
            using (var restRequest = new HttpRequestMessage(GetMethod(method), uri))
                return await SendInternalAsync(restRequest, headerOnly).ConfigureAwait(false);
        }
        public async Task<Stream> SendAsync(string method, string endpoint, string json, bool headerOnly = false)
        {
            string uri = Path.Combine(_baseUrl, endpoint);
            using (var restRequest = new HttpRequestMessage(GetMethod(method), uri))
            {
                restRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
                return await SendInternalAsync(restRequest, headerOnly).ConfigureAwait(false);
            }
        }
        public async Task<Stream> SendAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams, bool headerOnly = false)
        {
            string uri = Path.Combine(_baseUrl, endpoint);
            using (var restRequest = new HttpRequestMessage(GetMethod(method), uri))
            {
                var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
                if (multipartParams != null)
                {
                    foreach (var p in multipartParams)
                    {
                        //TODO: C#7 Typeswitch candidate
                        var stringValue = p.Value as string;
                        if (stringValue != null) { content.Add(new StringContent(stringValue), p.Key); continue; }
                        var byteArrayValue = p.Value as byte[];
                        if (byteArrayValue != null) { content.Add(new ByteArrayContent(byteArrayValue), p.Key); continue; }
                        var streamValue = p.Value as Stream;
                        if (streamValue != null) { content.Add(new StreamContent(streamValue), p.Key); continue; }
                        if (p.Value is MultipartFile)
                        {
                            var fileValue = (MultipartFile)p.Value;
                            content.Add(new StreamContent(fileValue.Stream), p.Key, fileValue.Filename);
                            continue;
                        }

                        throw new InvalidOperationException($"Unsupported param type \"{p.Value.GetType().Name}\"");
                    }
                }
                restRequest.Content = content;
                return await SendInternalAsync(restRequest, headerOnly).ConfigureAwait(false);
            }
        }

        private async Task<Stream> SendInternalAsync(HttpRequestMessage request, bool headerOnly)
        {
            while (true)
            {
                var cancelToken = _cancelToken; //It's okay if another thread changes this, causes a retry to abort
                HttpResponseMessage response = await _client.SendAsync(request, cancelToken).ConfigureAwait(false);

                int statusCode = (int)response.StatusCode;
                if (statusCode < 200 || statusCode >= 300) //2xx = Success
                {
                    string reason = null;
                    JToken content = null;
                    if (response.Content.Headers.GetValues("content-type").FirstOrDefault() == "application/json")
                    {
                        try
                        {
                            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            using (var reader = new StreamReader(stream))
                            using (var json = new JsonTextReader(reader))
                            {
                                content = _errorDeserializer.Deserialize<JToken>(json);
                                reason = content.Value<string>("message");
                                if (reason == null) //Occasionally an error message is given under a different key because reasons
                                    reason = content.ToString(Formatting.None);
                            }
                        }
                        catch { } //Might have been HTML Should we check for content-type?
                    }

                    if (statusCode == 429 && content != null)
                    {
                        //TODO: Include bucket info
                        string bucketId = content.Value<string>("bucket");
                        int retryAfterMillis = content.Value<int>("retry_after");
                        throw new HttpRateLimitException(bucketId, retryAfterMillis, reason);
                    }
                    else
                        throw new HttpException(response.StatusCode, reason);
                }

                if (headerOnly)
                    return null;
                else
                    return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
        }

        private static readonly HttpMethod _patch = new HttpMethod("PATCH");
        private HttpMethod GetMethod(string method)
        {
            switch (method)
            {
                case "DELETE": return HttpMethod.Delete;
                case "GET": return HttpMethod.Get;
                case "PATCH": return _patch;
                case "POST": return HttpMethod.Post;
                case "PUT": return HttpMethod.Put;
                default: throw new ArgumentOutOfRangeException(nameof(method), $"Unknown HttpMethod: {method}");
            }
        }
    }
}
