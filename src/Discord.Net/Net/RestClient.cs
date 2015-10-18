using Discord.API;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net
{
	internal partial class RestClient
	{
		private readonly DiscordAPIClientConfig _config;
		private CancellationToken _cancelToken;

		public RestClient(DiscordAPIClientConfig config)
		{
			_config = config;
            Initialize();
        }
		partial void Initialize();

		//DELETE
		private static readonly HttpMethod _delete = HttpMethod.Delete;
		internal Task<ResponseT> Delete<ResponseT>(string path, object data) where ResponseT : class  
			=> Send<ResponseT>(_delete, path, data);
		internal Task<ResponseT> Delete<ResponseT>(string path) where ResponseT : class  
			=> Send<ResponseT>(_delete, path);
		internal Task Delete(string path, object data) 
			=> Send(_delete, path, data);
		internal Task Delete(string path) 
			=> Send(_delete, path);

		//GET
		private static readonly HttpMethod _get = HttpMethod.Get;
		internal Task<ResponseT> Get<ResponseT>(string path) where ResponseT : class 
			=> Send<ResponseT>(_get, path);
		internal Task Get(string path) 
			=> Send(_get, path);

		//PATCH
		private static readonly HttpMethod _patch = new HttpMethod("PATCH");
		internal Task<ResponseT> Patch<ResponseT>(string path, object data) where ResponseT : class 
			=> Send<ResponseT>(_patch, path, data);
		internal Task Patch(string path, object data) 
			=> Send(_patch, path, data);

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

			if (_config.LogLevel >= LogMessageSeverity.Verbose)
				stopwatch = Stopwatch.StartNew();
			
			string responseJson = await SendInternal(method, path, requestJson, _cancelToken).ConfigureAwait(false);

#if TEST_RESPONSES
			if (!hasResponse && !string.IsNullOrEmpty(responseJson))
				throw new Exception("API check failed: Response is not empty.");
#endif

			if (_config.LogLevel >= LogMessageSeverity.Verbose)
			{
				stopwatch.Stop();
				if (content != null && _config.LogLevel >= LogMessageSeverity.Debug)
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

			if (_config.LogLevel >= LogMessageSeverity.Verbose)
				stopwatch = Stopwatch.StartNew();
			
			string responseJson = await SendFileInternal(method, path, filePath, _cancelToken).ConfigureAwait(false);

#if TEST_RESPONSES
			if (!hasResponse && !string.IsNullOrEmpty(responseJson))
				throw new Exception("API check failed: Response is not empty.");
#endif

			if (_config.LogLevel >= LogMessageSeverity.Verbose)
			{
				stopwatch.Stop();
				if (_config.LogLevel >= LogMessageSeverity.Debug)
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

		internal void SetCancelToken(CancellationToken token) => _cancelToken = token;
	}
}
