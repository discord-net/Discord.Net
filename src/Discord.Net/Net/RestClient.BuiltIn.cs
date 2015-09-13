#if DNXCORE50
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net
{
	internal class BuiltInRestEngine : IRestEngine
	{
		private readonly HttpClient _client;

		public BuiltInRestEngine(string userAgent)
		{
			_client = new HttpClient(new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
				UseCookies = false,
				PreAuthenticate = false //We do auth ourselves
			});
			_client.DefaultRequestHeaders.Add("accept", "*/*");
			_client.DefaultRequestHeaders.Add("accept-encoding", "gzip,deflate");			
			_client.DefaultRequestHeaders.Add("user-agent", userAgent);
		}

		public void SetToken(string token)
		{
			_client.DefaultRequestHeaders.Remove("authorization");
			if (token != null)
				_client.DefaultRequestHeaders.Add("authorization", token);
		}

		public Task<string> Send(HttpMethod method, string path, string json, CancellationToken cancelToken)
		{
			using (var request = new HttpRequestMessage(method, path))
			{
				if (json != null)
					request.Content = new StringContent(json, Encoding.UTF8, "application/json");
				return Send(request, cancelToken);
			}
		}
		public Task<string> SendFile(HttpMethod method, string path, string filePath, CancellationToken cancelToken)
		{
			using (var request = new HttpRequestMessage(method, path))
			{
				var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
				content.Add(new StreamContent(File.OpenRead(filePath)), "file", Path.GetFileName(filePath));
				request.Content = content;
				return Send(request, cancelToken);
			}
		}
		private async Task<string> Send(HttpRequestMessage request, CancellationToken cancelToken)
		{
			var response = await _client.SendAsync(request, cancelToken).ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
				throw new HttpException(response.StatusCode);
			return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
		}
	}
}
#endif