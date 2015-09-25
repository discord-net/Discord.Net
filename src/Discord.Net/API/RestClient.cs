using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API
{
	internal interface IRestEngine
	{
		void SetToken(string token);
		Task<string> Send(HttpMethod method, string path, string json, CancellationToken cancelToken);
		Task<string> SendFile(HttpMethod method, string path, string filePath, CancellationToken cancelToken);
    }

	internal partial class RestClient
	{
		private readonly IRestEngine _engine;
		private readonly LogMessageSeverity _logLevel;
		private CancellationToken _cancelToken;

		public RestClient(LogMessageSeverity logLevel, int timeout)
		{
			_logLevel = logLevel;
			
			string version = typeof(RestClient).GetTypeInfo().Assembly.GetName().Version.ToString(2);
			string userAgent = $"Discord.Net/{version} (https://github.com/RogueException/Discord.Net)";
#if DNXCORE50
			_engine = new BuiltInRestEngine(userAgent, timeout);
#else
			_engine = new RestSharpRestEngine(userAgent, timeout);
#endif
		}

		private static readonly HttpMethod _delete = HttpMethod.Delete;
		internal Task<ResponseT> Delete<ResponseT>(string path, object data) where ResponseT : class  
			=> Send<ResponseT>(_delete, path, data);
		internal Task<ResponseT> Delete<ResponseT>(string path) where ResponseT : class  
			=> Send<ResponseT>(_delete, path);
		internal Task Delete(string path, object data) 
			=> Send(_delete, path, data);
		internal Task Delete(string path) 
			=> Send(_delete, path);

		private static readonly HttpMethod _get = HttpMethod.Get;
		internal Task<ResponseT> Get<ResponseT>(string path) where ResponseT : class 
			=> Send<ResponseT>(_get, path);
		internal Task Get(string path) 
			=> Send(_get, path);

		private static readonly HttpMethod _patch = new HttpMethod("PATCH");
		internal Task<ResponseT> Patch<ResponseT>(string path, object data) where ResponseT : class 
			=> Send<ResponseT>(_patch, path, data);
		internal Task Patch(string path, object data) 
			=> Send(_patch, path, data);
		internal Task<ResponseT> Patch<ResponseT>(string path) where ResponseT : class 
			=> Send<ResponseT>(_patch, path);
		internal Task Patch(string path) 
			=> Send(_patch, path);

		private static readonly HttpMethod _post = HttpMethod.Post;
		internal Task<ResponseT> Post<ResponseT>(string path, object data) where ResponseT : class 
			=> Send<ResponseT>(_post, path, data);
		internal Task<ResponseT> Post<ResponseT>(string path) where ResponseT : class 
			=> Send<ResponseT>(_post, path);
		internal Task Post(string path, object data) 
			=> Send(_post, path, data);
		internal Task Post(string path) 
			=> Send(_post, path);

		private static readonly HttpMethod _put = HttpMethod.Put;
		internal Task<ResponseT> Put<ResponseT>(string path, object data) where ResponseT : class 
			=> Send<ResponseT>(_put, path, data);
		internal Task<ResponseT> Put<ResponseT>(string path) where ResponseT : class 
			=> Send<ResponseT>(_put, path);
		internal Task Put(string path, object data) 
			=> Send(_put, path, data);
		internal Task Put(string path) 
			=> Send(_put, path);

		internal Task<ResponseT> PostFile<ResponseT>(string path, string filePath) where ResponseT : class 
			=> SendFile<ResponseT>(_post, path, filePath);
		internal Task PostFile(string path, string filePath) 
			=> SendFile(_post, path, filePath);

		internal Task<ResponseT> PutFile<ResponseT>(string path, string filePath) where ResponseT : class 
			=> SendFile<ResponseT>(_put, path, filePath);
		internal Task PutFile(string path, string filePath) 
			=> SendFile(_put, path, filePath);

		private async Task<ResponseT> Send<ResponseT>(HttpMethod method, string path, object content = null)
			where ResponseT : class
		{
			string responseJson = await Send(method, path, content, true).ConfigureAwait(false);
			return DeserializeResponse<ResponseT>(responseJson);
		}
		private Task Send(HttpMethod method, string path, object content = null)
			=> Send(method, path, content, false);
		private async Task<string> Send(HttpMethod method, string path, object content, bool hasResponse)
		{
			Stopwatch stopwatch = null;
			string requestJson = null;
			if (content != null)
				requestJson = JsonConvert.SerializeObject(content);

			if (_logLevel >= LogMessageSeverity.Verbose)
				stopwatch = Stopwatch.StartNew();
			
			string responseJson = await _engine.Send(method, path, requestJson, _cancelToken).ConfigureAwait(false);

#if TEST_RESPONSES
			if (!hasResponse && !string.IsNullOrEmpty(responseJson))
				throw new Exception("API check failed: Response is not empty.");
#endif

			if (_logLevel >= LogMessageSeverity.Verbose)
			{
				stopwatch.Stop();
				if (content != null && _logLevel >= LogMessageSeverity.Debug)
				{
					if (path.StartsWith(Endpoints.Auth))
                        RaiseOnRequest(method, path, "[Hidden]", stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond);
					else
						RaiseOnRequest(method, path, requestJson, stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond);
				}
				else
					RaiseOnRequest(method, path, null, stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond);
			}

			return responseJson;
		}

		private async Task<ResponseT> SendFile<ResponseT>(HttpMethod method, string path, string filePath)
			where ResponseT : class
		{
			string responseJson = await SendFile(method, path, filePath, true).ConfigureAwait(false);
			return DeserializeResponse<ResponseT>(responseJson);
		}
		private Task SendFile(HttpMethod method, string path, string filePath)
			=> SendFile(method, path, filePath, false);
		private async Task<string> SendFile(HttpMethod method, string path, string filePath, bool hasResponse)
		{
			Stopwatch stopwatch = null;

			if (_logLevel >= LogMessageSeverity.Verbose)
				stopwatch = Stopwatch.StartNew();
			
			string responseJson = await _engine.SendFile(method, path, filePath, _cancelToken).ConfigureAwait(false);

#if TEST_RESPONSES
			if (!hasResponse && !string.IsNullOrEmpty(responseJson))
				throw new Exception("API check failed: Response is not empty.");
#endif

			if (_logLevel >= LogMessageSeverity.Verbose)
			{
				stopwatch.Stop();
				if (_logLevel >= LogMessageSeverity.Debug)
					RaiseOnRequest(method, path, filePath, stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond);
				else
                    RaiseOnRequest(method, path, null, stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond);
			}

			return responseJson;
		}

		private JsonSerializerSettings _deserializeSettings = new JsonSerializerSettings();
		private T DeserializeResponse<T>(string json)
		{
#if TEST_RESPONSES
			if (_deserializeSettings == null)
			{
				_deserializeSettings = new JsonSerializerSettings();
				_deserializeSettings.CheckAdditionalContent = true;
				_deserializeSettings.MissingMemberHandling = MissingMemberHandling.Error;
			}
			if (string.IsNullOrEmpty(json))
				throw new Exception("API check failed: Response is empty.");
			return JsonConvert.DeserializeObject<T>(json, _deserializeSettings);
#else
			return JsonConvert.DeserializeObject<T>(json);
#endif
		}

		internal void SetToken(string token) => _engine.SetToken(token);
		internal void SetCancelToken(CancellationToken token) => _cancelToken = token;
	}
}
