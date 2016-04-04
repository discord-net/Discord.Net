using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    public class DefaultRestEngine : IRestEngine
    {
        private const int HR_SECURECHANNELFAILED = -2146233079;

        protected readonly HttpClient _client;
        protected readonly string _baseUrl;
        protected readonly CancellationToken _cancelToken;
        protected bool _isDisposed;

        public DefaultRestEngine(string baseUrl, CancellationToken cancelToken)
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
            SetHeader("accept-encoding", "gzip,deflate");
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

        public async Task<Stream> Send(IRestRequest request)
        {
            string uri = Path.Combine(_baseUrl, request.Endpoint);
            using (var restRequest = new HttpRequestMessage(GetMethod(request.Method), uri))
            {
                object payload = request.Payload;
                if (payload != null)
                    restRequest.Content =  new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                return await SendInternal(restRequest, _cancelToken).ConfigureAwait(false);
            }
        }

        public async Task<Stream> Send(IRestFileRequest request)
        {
            string uri = Path.Combine(_baseUrl, request.Endpoint);
            using (var restRequest = new HttpRequestMessage(GetMethod(request.Method), uri))
            {
                var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
                var mpParameters = request.MultipartParameters;
                if (mpParameters != null)
                {
                    foreach (var p in mpParameters)
                        content.Add(new StringContent(p.Value), p.Key);

                }
                content.Add(new StreamContent(request.Stream), "file", request.Filename);
                restRequest.Content = content;
                return await SendInternal(restRequest, _cancelToken).ConfigureAwait(false);
            }
        }

        private async Task<Stream> SendInternal(HttpRequestMessage request, CancellationToken cancelToken)
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
                    throw new HttpException(response.StatusCode);

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
