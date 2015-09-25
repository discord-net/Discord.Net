#if !DNXCORE50
using RestSharp;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class RestSharpRestEngine : IRestEngine
	{
		private readonly RestSharp.RestClient _client;

		public RestSharpRestEngine(string userAgent, int timeout)
		{
			_client = new RestSharp.RestClient(Endpoints.BaseApi)
			{
				PreAuthenticate = false
			};
			_client.AddDefaultHeader("accept", "*/*");
			_client.AddDefaultHeader("accept-encoding", "gzip,deflate");
            _client.UserAgent = userAgent;
			_client.ReadWriteTimeout = timeout;
		}

		public void SetToken(string token)
		{
			_client.RemoveDefaultParameter("authorization");
			if (token != null)
				_client.AddDefaultHeader("authorization", token);
		}

		public Task<string> Send(HttpMethod method, string path, string json, CancellationToken cancelToken)
		{
			var request = new RestRequest(path, GetMethod(method));
			request.AddParameter("application/json", json, ParameterType.RequestBody);
			return Send(request, cancelToken);
		}
		public Task<string> SendFile(HttpMethod method, string path, string filePath, CancellationToken cancelToken)
		{
			var request = new RestRequest(path, Method.POST);
            request.AddFile(Path.GetFileName(filePath), filePath);
			return Send(request, cancelToken);
		}
		private async Task<string> Send(RestRequest request, CancellationToken cancelToken)
		{
			bool hasRetried = false;
			while (true)
			{
				var response = await _client.ExecuteTaskAsync(request, cancelToken).ConfigureAwait(false);
				int statusCode = (int)response.StatusCode;
				if (statusCode == 0) //Internal Error
				{
					if (!hasRetried)
					{
						//SSL/TTS Error seems to work if we immediately retry
						hasRetried = true;
						continue;
					}
					throw response.ErrorException;
				}
				if (statusCode < 200 || statusCode >= 300) //2xx = Success
					throw new HttpException(response.StatusCode);
				return response.Content;
			}
		}

		private Method GetMethod(HttpMethod method)
		{
			switch (method.Method)
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