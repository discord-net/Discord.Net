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
    public class DefaultRestClient : IRestClient
    {
        private const int HR_SECURECHANNELFAILED = -2146233079;

        protected readonly HttpClient _client;
        protected readonly string _baseUrl;
        protected readonly CancellationToken _cancelToken;
        protected bool _isDisposed;

        public DefaultRestClient(string baseUrl, CancellationToken cancelToken)
        {
            _baseUrl = baseUrl;
            _cancelToken = cancelToken;

            _client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = false,
                UseProxy = false,
                PreAuthenticate = false
            });

            SetHeader("accept-encoding", "gzip, deflate");
        }
        protected virtual void Dispose(bool disposing)
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
            _client.DefaultRequestHeaders.Add(key, value);
        }

        public async Task<Stream> Send(string method, string endpoint, string json = null, bool headerOnly = false)
        {
            string uri = Path.Combine(_baseUrl, endpoint);
            using (var restRequest = new HttpRequestMessage(GetMethod(method), uri))
            {
                if (json != null)
                    restRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
                return await SendInternal(restRequest, _cancelToken, headerOnly).ConfigureAwait(false);
            }
        }

        public async Task<Stream> Send(string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams, bool headerOnly = false)
        {
            string uri = Path.Combine(_baseUrl, endpoint);
            using (var restRequest = new HttpRequestMessage(GetMethod(method), uri))
            {
                var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
                if (multipartParams != null)
                {
                    foreach (var p in multipartParams)
                    {
                        switch (p.Value)
                        {
                            case string value:
                                content.Add(new StringContent(value), p.Key);
                                break;
                            case byte[] value:
                                content.Add(new ByteArrayContent(value), p.Key);
                                break;
                            case Stream value:
                                content.Add(new StreamContent(value), p.Key);
                                break;
                            case MultipartFile value:
                                content.Add(new StreamContent(value.Stream), value.Filename, p.Key);
                                break;
                            default:
                                throw new InvalidOperationException($"Unsupported param type \"{p.Value.GetType().Name}\"");
                        }
                    }
                }
                restRequest.Content = content;
                return await SendInternal(restRequest, _cancelToken, headerOnly).ConfigureAwait(false);
            }
        }

        private async Task<Stream> SendInternal(HttpRequestMessage request, CancellationToken cancelToken, bool headerOnly)
        {
            int retryCount = 0;
            while (true)
            {
                HttpResponseMessage response;
                try
                {
                    response = await _client.SendAsync(request, cancelToken).ConfigureAwait(false);
                }
                catch (WebException ex)
                {
                    //The request was aborted: Could not create SSL/TLS secure channel.
                    if (ex.HResult == HR_SECURECHANNELFAILED && retryCount++ < 5)
                        continue; //Retrying seems to fix this somehow?
                    throw;
                }

                int statusCode = (int)response.StatusCode;
                if (statusCode < 200 || statusCode >= 300) //2xx = Success
                {
                    if (statusCode == 429)
                        throw new HttpRateLimitException(int.Parse(response.Headers.GetValues("retry-after").First()));
                    throw new HttpException(response.StatusCode);
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
