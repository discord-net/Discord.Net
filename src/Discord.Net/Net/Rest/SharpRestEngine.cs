#if !DOTNET5_4
using Discord.Logging;
using Nito.AsyncEx;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RestSharpClient = RestSharp.RestClient;

namespace Discord.Net.Rest
{
    internal class RestSharpEngine : IRestEngine
	{
        private const int HR_SECURECHANNELFAILED = -2146233079;

        private readonly DiscordConfig _config;
		private readonly RestSharpClient _client;

        private readonly AsyncLock _rateLimitLock;
        private readonly ILogger _logger;
        private DateTime _rateLimitTime;

        public RestSharpEngine(DiscordConfig config, string baseUrl, ILogger logger)
		{
			_config = config;
            _logger = logger;

            _rateLimitLock = new AsyncLock();
            _client = new RestSharpClient(baseUrl)
			{
				PreAuthenticate = false,
				ReadWriteTimeout = DiscordConfig.RestTimeout,
				UserAgent = config.UserAgent
            };
			_client.Proxy = null;
            _client.RemoveDefaultParameter("Accept");
            _client.AddDefaultHeader("accept", "*/*");
			_client.AddDefaultHeader("accept-encoding", "gzip,deflate");
        }

		public void SetToken(string token)
		{
			_client.RemoveDefaultParameter("authorization");
			if (token != null)
				_client.AddDefaultHeader("authorization", token);
		}

		public Task<string> Send(string method, string path, string json, CancellationToken cancelToken)
		{
			var request = new RestRequest(path, GetMethod(method));
            if (json != null)
			    request.AddParameter("application/json", json, ParameterType.RequestBody);
			return Send(request, cancelToken);
		}
		public Task<string> SendFile(string method, string path, string filename, Stream stream, CancellationToken cancelToken)
		{
			var request = new RestRequest(path, GetMethod(method));
			request.AddHeader("content-length", (stream.Length - stream.Position).ToString());

			byte[] bytes = new byte[stream.Length - stream.Position];
			stream.Read(bytes, 0, bytes.Length);
			request.AddFileBytes("file", bytes, filename);
			//request.AddFile("file", x => stream.CopyTo(x), filename); (Broken in latest ver)

			return Send(request, cancelToken);
		}
		private async Task<string> Send(RestRequest request, CancellationToken cancelToken)
        {
            int retryCount = 0;
            while (true)
            {
                var response = await _client.ExecuteTaskAsync(request, cancelToken).ConfigureAwait(false);
                int statusCode = (int)response.StatusCode;
                if (statusCode == 0) //Internal Error
                {
                    //The request was aborted: Could not create SSL/TLS secure channel.
                    if (response.ErrorException.HResult == HR_SECURECHANNELFAILED && retryCount++ < 5)
                        continue; //Retrying seems to fix this somehow?
                    throw response.ErrorException;
                }
                else if (statusCode == 429) //Rate limit
                {
                    var retryAfter = response.Headers
                        .FirstOrDefault(x => x.Name.Equals("Retry-After", StringComparison.OrdinalIgnoreCase));

                    int milliseconds;
                    if (retryAfter != null && int.TryParse((string)retryAfter.Value, out milliseconds))
                    {
                        if (_logger != null)
                        {
                            var now = DateTime.UtcNow;
                            if (now >= _rateLimitTime)
                            {
                                using (await _rateLimitLock.LockAsync().ConfigureAwait(false))
                                {
                                    if (now >= _rateLimitTime)
                                    {
                                        _rateLimitTime = now.AddMilliseconds(milliseconds);
                                        _logger.Warning($"Rate limit hit, waiting {Math.Round(milliseconds / 1000.0f, 2)} seconds");
                                    }
                                }
                            }
                        }
                        await Task.Delay(milliseconds, cancelToken).ConfigureAwait(false);
                        continue;
                    }
                    throw new HttpException(response.StatusCode);
                }
                else if (statusCode < 200 || statusCode >= 300) //2xx = Success
                    throw new HttpException(response.StatusCode);
                else
                    return response.Content;
            }
		}

		private Method GetMethod(string method)
		{
			switch (method)
			{
				case "DELETE": return Method.DELETE;
				case "GET": return Method.GET;
				case "PATCH": return Method.PATCH;
				case "POST": return Method.POST;
				case "PUT": return Method.PUT;
				default: throw new InvalidOperationException($"Unknown HttpMethod: {method}");
			}
		}
	}
}
#endif