#if !DNXCORE50
using RestSharp;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net
{
    internal class SharpRestEngine : IRestEngine
	{
		private readonly RestSharp.RestClient _client;

		public SharpRestEngine(string userAgent)
		{
			_client = new RestSharp.RestClient()
			{
				PreAuthenticate = false
			};
			_client.AddDefaultHeader("accept", "*/*");
			_client.AddDefaultHeader("accept-encoding", "gzip,deflate");
			_client.UserAgent = userAgent;
		}

		public void SetToken(string token)
		{
			_client.RemoveDefaultParameter("authorization");
			if (token != null)
				_client.AddDefaultHeader("authorization", token);
		}

		public Task<string> Send(HttpMethod method, string path, string json, CancellationToken cancelToken)
		{
			var request = new RestRequest(path, GetMethod(method)) { RequestFormat = DataFormat.Json };
			request.AddBody(json);
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
			var response = await _client.ExecuteTaskAsync(request, cancelToken).ConfigureAwait(false);
			int statusCode = (int)response.StatusCode;
			if (statusCode < 200 || statusCode >= 300) //2xx = Success
				throw new HttpException(response.StatusCode);
			return response.Content;
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