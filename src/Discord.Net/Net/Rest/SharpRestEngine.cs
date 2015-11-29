#if !DOTNET5_4
using Discord.API;
using RestSharp;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    internal sealed class RestSharpEngine : IRestEngine
	{
		private readonly DiscordAPIClientConfig _config;
		private readonly RestSharp.RestClient _client;

        public RestSharpEngine(DiscordAPIClientConfig config)
		{
			_config = config;
			_client = new RestSharp.RestClient(Endpoints.BaseApi)
			{
				PreAuthenticate = false,
				ReadWriteTimeout = _config.APITimeout,
				UserAgent = _config.UserAgent
			};
			if (_config.ProxyUrl != null)
				_client.Proxy = new WebProxy(_config.ProxyUrl, true, new string[0], _config.ProxyCredentials);
			else
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
			request.AddParameter("application/json", json, ParameterType.RequestBody);
			return Send(request, cancelToken);
		}
		public Task<string> SendFile(string method, string path, string filePath, CancellationToken cancelToken)
		{
			var request = new RestRequest(path, Method.POST);
            request.AddFile("file", filePath);
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
					if (response.ErrorException.HResult == -2146233079 && retryCount++ < 5) //The request was aborted: Could not create SSL/TLS secure channel.
						continue; //Seems to work if we immediately retry
					throw response.ErrorException;
				}
				if (statusCode < 200 || statusCode >= 300) //2xx = Success
					throw new HttpException(response.StatusCode);
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