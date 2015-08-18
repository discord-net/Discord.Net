using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Reflection;

namespace Discord.Helpers
{
	internal static class Http
	{
#if DEBUG
		private const bool _isDebug = true;
#else
		private const bool _isDebug = false;
#endif
		private static readonly HttpClient _client;
		private static readonly HttpMethod _patch = new HttpMethod("PATCH"); //Not sure why this isn't a default...

		static Http()
		{
			_client = new HttpClient();
			_client.DefaultRequestHeaders.Add("accept", "*/*");
			_client.DefaultRequestHeaders.Add("accept-language", "en-US;q=0.8");

			string version = typeof(Http).GetTypeInfo().Assembly.GetName().Version.ToString(2);
			_client.DefaultRequestHeaders.Add("user-agent", $"Discord.Net/{version} (https://github.com/RogueException/Discord.Net)");
		}

		private static string _token;
		public static string Token
		{
			get { return _token; }
			set
			{
				_token = value;
				_client.DefaultRequestHeaders.Remove("authorization");
				if (_token != null)
					_client.DefaultRequestHeaders.Add("authorization", _token);
			}
		}

		internal static Task<ResponseT> Get<ResponseT>(string path)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Get, path, null);
		internal static Task<string> Get(string path)
			=> Send(HttpMethod.Get, path, null);
		
		internal static Task<ResponseT> Post<ResponseT>(string path, object data)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Post, path, data);
		internal static Task<string> Post(string path, object data)
			=> Send(HttpMethod.Post, path, data);
		internal static Task<ResponseT> Post<ResponseT>(string path)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Post, path, null);
		internal static Task<string> Post(string path)
			=> Send(HttpMethod.Post, path, null);
		
		internal static Task<ResponseT> Put<ResponseT>(string path, object data)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Put, path, data);
		internal static Task<string> Put(string path, object data)
			=> Send(HttpMethod.Put, path, data);
		internal static Task<ResponseT> Put<ResponseT>(string path)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Put, path, null);
		internal static Task<string> Put(string path)
			=> Send(HttpMethod.Put, path, null);

		internal static Task<ResponseT> Patch<ResponseT>(string path, object data)
			where ResponseT : class
			=> Send<ResponseT>(_patch, path, data);
		internal static Task<string> Patch(string path, object data)
			=> Send(_patch, path, data);
		internal static Task<ResponseT> Patch<ResponseT>(string path)
			where ResponseT : class
			=> Send<ResponseT>(_patch, path, null);
		internal static Task<string> Patch(string path)
			=> Send(_patch, path, null);

		internal static Task<ResponseT> Delete<ResponseT>(string path, object data)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Delete, path, data);
		internal static Task<string> Delete(string path, object data)
			=> Send(HttpMethod.Delete, path, data);
		internal static Task<ResponseT> Delete<ResponseT>(string path)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Delete, path, null);
		internal static Task<string> Delete(string path)
			=> Send(HttpMethod.Delete, path, null);

		internal static async Task<ResponseT> Send<ResponseT>(HttpMethod method, string path, object data)
			where ResponseT : class
		{
			string requestJson = data != null ? JsonConvert.SerializeObject(data) : null;
			string responseJson = await SendRequest(method, path, requestJson, true);
			var response = JsonConvert.DeserializeObject<ResponseT>(responseJson);
#if DEBUG
			CheckResponse(responseJson, response);
#endif
			return response;
		}
		internal static async Task<string> Send(HttpMethod method, string path, object data)
		{
			string requestJson = data != null ? JsonConvert.SerializeObject(data) : null;
			string responseJson = await SendRequest(method, path, requestJson, _isDebug);
#if DEBUG
			CheckEmptyResponse(responseJson);
#endif
			return responseJson;
		}

		private static async Task<string> SendRequest(HttpMethod method, string path, string data, bool hasResponse)
		{
			//Create Request
			HttpRequestMessage msg = new HttpRequestMessage(method, path);

			//Add Payload
			if (data != null)
				msg.Content = new StringContent(data, Encoding.UTF8, "application/json");

			if (!hasResponse)
			{
				await _client.SendAsync(msg, HttpCompletionOption.ResponseHeadersRead);
				return null;
			}
			else
			{
				var response = await _client.SendAsync(msg, HttpCompletionOption.ResponseContentRead);
				return await response.Content.ReadAsStringAsync();
            }
		}

#if DEBUG
		private static void CheckResponse<T>(string json, T obj)
		{
			/*JToken token = JToken.Parse(json);
			JToken token2 = JToken.FromObject(obj);
			if (!JToken.DeepEquals(token, token2))
				throw new Exception("API check failed: Objects do not match.");*/
		}

		private static void CheckEmptyResponse(string json)
		{
			if (!string.IsNullOrEmpty(json))
				throw new Exception("API check failed: Response is not empty.");
		}
#endif
	}
}
