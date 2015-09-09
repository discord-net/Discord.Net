using Discord.API;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Globalization;
using System.Net.Http;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Discord.Helpers
{
	internal partial class JsonHttpClient
	{
		private bool _isDebug;
		private readonly HttpClient _client;
		private readonly HttpMethod _patch;
#if TEST_RESPONSES
		private readonly JsonSerializerSettings _settings;
#endif

		public JsonHttpClient(bool isDebug)
		{
			_isDebug = isDebug;
			_patch = new HttpMethod("PATCH"); //Not sure why this isn't a default...

			_client = new HttpClient(new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
				UseCookies = false,
				PreAuthenticate = false //We do auth ourselves
			});
			_client.DefaultRequestHeaders.Add("accept", "*/*");
			_client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate");

			string version = typeof(JsonHttpClient).GetTypeInfo().Assembly.GetName().Version.ToString(2);
			_client.DefaultRequestHeaders.Add("user-agent", $"Discord.Net/{version} (https://github.com/RogueException/Discord.Net)");

#if TEST_RESPONSES
			_settings = new JsonSerializerSettings();
			_settings.CheckAdditionalContent = true;
			_settings.MissingMemberHandling = MissingMemberHandling.Error;
#endif
		}

		private string _token;
		public string Token
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

		internal Task<ResponseT> Get<ResponseT>(string path)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Get, path, null);
		internal Task<string> Get(string path)
			=> Send(HttpMethod.Get, path, null);
		
		internal Task<ResponseT> Post<ResponseT>(string path, object data)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Post, path, AsJson(data));
		internal Task<string> Post(string path, object data)
			=> Send(HttpMethod.Post, path, AsJson(data));
		internal Task<ResponseT> Post<ResponseT>(string path)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Post, path, null);
		internal Task<string> Post(string path)
			=> Send(HttpMethod.Post, path, null);
		
		internal Task<ResponseT> Put<ResponseT>(string path, object data)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Put, path, AsJson(data));
		internal Task<string> Put(string path, object data)
			=> Send(HttpMethod.Put, path, AsJson(data));
		internal Task<ResponseT> Put<ResponseT>(string path)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Put, path, null);
		internal Task<string> Put(string path)
			=> Send(HttpMethod.Put, path, null);

		internal Task<ResponseT> Patch<ResponseT>(string path, object data)
			where ResponseT : class
			=> Send<ResponseT>(_patch, path, AsJson(data));
		internal Task<string> Patch(string path, object data)
			=> Send(_patch, path, AsJson(data));
		internal Task<ResponseT> Patch<ResponseT>(string path)
			where ResponseT : class
			=> Send<ResponseT>(_patch, path, null);
		internal Task<string> Patch(string path)
			=> Send(_patch, path, null);

		internal Task<ResponseT> Delete<ResponseT>(string path, object data)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Delete, path, AsJson(data));
		internal Task<string> Delete(string path, object data)
			=> Send(HttpMethod.Delete, path, AsJson(data));
		internal Task<ResponseT> Delete<ResponseT>(string path)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Delete, path, null);
		internal Task<string> Delete(string path)
			=> Send(HttpMethod.Delete, path, null);

		internal Task<ResponseT> File<ResponseT>(string path, Stream stream, string filename = null)
			where ResponseT : class
			=> Send<ResponseT>(HttpMethod.Post, path, AsFormData(stream, filename));
		internal Task<string> File(string path, Stream stream, string filename = null)
			=> Send(HttpMethod.Post, path, AsFormData(stream, filename));

		private async Task<ResponseT> Send<ResponseT>(HttpMethod method, string path, HttpContent content)
			where ResponseT : class
		{
			string responseJson = await SendRequest(method, path, content, true).ConfigureAwait(false);
#if TEST_RESPONSES
			if (path.StartsWith(Endpoints.BaseApi))
				return JsonConvert.DeserializeObject<ResponseT>(responseJson, _settings);
#endif
			return JsonConvert.DeserializeObject<ResponseT>(responseJson);
		}
#if TEST_RESPONSES
		private async Task<string> Send(HttpMethod method, string path, HttpContent content)
		{
			string responseJson = await SendRequest(method, path, content, true).ConfigureAwait(false);
			if (path.StartsWith(Endpoints.BaseApi) && !string.IsNullOrEmpty(responseJson))
				throw new Exception("API check failed: Response is not empty.");
			return responseJson;
		}
#else
		private Task<string> Send(HttpMethod method, string path, HttpContent content)
			=> SendRequest(method, path, content, false);
#endif

		private async Task<string> SendRequest(HttpMethod method, string path, HttpContent content, bool hasResponse)
		{
			Stopwatch stopwatch = null;
			if (_isDebug)
			{
				if (content != null)
				{
					if (content is StringContent)
					{
						string json = await (content as StringContent).ReadAsStringAsync().ConfigureAwait(false);
						RaiseOnDebugMessage(DebugMessageType.XHRRawOutput, $"{method} {path}: {json}");
					}
					else
					{
						byte[] bytes = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
						RaiseOnDebugMessage(DebugMessageType.XHRRawOutput, $"{method} {path}: {bytes.Length} bytes");
					}
				}
				stopwatch = Stopwatch.StartNew();
			}

			string result;
			using (HttpRequestMessage msg = new HttpRequestMessage(method, path))
			{
				if (content != null)
					msg.Content = content;

				HttpResponseMessage response;
				if (hasResponse)
				{
					response = await _client.SendAsync(msg, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
					if (!response.IsSuccessStatusCode)
						throw new HttpException(response.StatusCode);
					result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				}
				else
				{
#if !NET45
					response = await _client.SendAsync(msg, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
#else
					response = await _client.SendAsync(msg, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
#endif
					if (!response.IsSuccessStatusCode)
						throw new HttpException(response.StatusCode);
					result = null;
				}
			}

			if (_isDebug)
			{
				stopwatch.Stop();
				RaiseOnDebugMessage(DebugMessageType.XHRTiming, $"{method} {path}: {Math.Round(stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond, 2)}ms");
			}
			return result;
		}

		private StringContent AsJson(object obj)
		{
			return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
		}
		private MultipartFormDataContent AsFormData(Stream stream, string filename)
		{
			var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
			content.Add(new StreamContent(stream), "file", filename);
			return content;
		}
	}
}
