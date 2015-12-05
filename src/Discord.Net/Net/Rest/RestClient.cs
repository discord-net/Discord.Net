using Discord.API;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
	internal sealed partial class RestClient
	{
		private readonly DiscordConfig _config;
		private readonly IRestEngine _engine;
		private CancellationToken _cancelToken;

		public RestClient(DiscordConfig config)
		{
			_config = config;
#if !DOTNET5_4
			_engine = new RestSharpEngine(config);
#else
			//_engine = new BuiltInRestEngine(config);
#endif
		}

		public void SetToken(string token) => _engine.SetToken(token);

		//DELETE
		internal Task<ResponseT> Delete<ResponseT>(string path, object data) where ResponseT : class  
			=> Send<ResponseT>("DELETE", path, data);
		internal Task<ResponseT> Delete<ResponseT>(string path) where ResponseT : class  
			=> Send<ResponseT>("DELETE", path);
		internal Task Delete(string path, object data) 
			=> Send("DELETE", path, data);
		internal Task Delete(string path) 
			=> Send("DELETE", path);

		//GET
		internal Task<ResponseT> Get<ResponseT>(string path) where ResponseT : class 
			=> Send<ResponseT>("GET", path);
		internal Task Get(string path) 
			=> Send("GET", path);

		//PATCH
		internal Task<ResponseT> Patch<ResponseT>(string path, object data) where ResponseT : class 
			=> Send<ResponseT>("PATCH", path, data);
		internal Task Patch(string path, object data) 
			=> Send("PATCH", path, data);
		
		internal Task<ResponseT> Post<ResponseT>(string path, object data) where ResponseT : class 
			=> Send<ResponseT>("POST", path, data);
		internal Task<ResponseT> Post<ResponseT>(string path) where ResponseT : class 
			=> Send<ResponseT>("POST", path);
		internal Task Post(string path, object data) 
			=> Send("POST", path, data);
		internal Task Post(string path) 
			=> Send("POST", path);
		
		internal Task<ResponseT> Put<ResponseT>(string path, object data) where ResponseT : class 
			=> Send<ResponseT>("PUT", path, data);
		internal Task<ResponseT> Put<ResponseT>(string path) where ResponseT : class 
			=> Send<ResponseT>("PUT", path);
		internal Task Put(string path, object data) 
			=> Send("PUT", path, data);
		internal Task Put(string path) 
			=> Send("PUT", path);

		internal Task<ResponseT> PostFile<ResponseT>(string path, string filename, Stream stream) where ResponseT : class 
			=> SendFile<ResponseT>("POST", path, filename, stream);
		internal Task PostFile(string path, string filename, Stream stream) 
			=> SendFile("POST", path, filename, stream);

		internal Task<ResponseT> PutFile<ResponseT>(string path, string filename, Stream stream) where ResponseT : class 
			=> SendFile<ResponseT>("PUT", path, filename, stream);
		internal Task PutFile(string path, string filename, Stream stream) 
			=> SendFile("PUT", path, filename, stream);

		private async Task<ResponseT> Send<ResponseT>(string method, string path, object content = null)
			where ResponseT : class
		{
			string responseJson = await Send(method, path, content, true).ConfigureAwait(false);
			return DeserializeResponse<ResponseT>(responseJson);
		}
		private Task Send(string method, string path, object content = null)
			=> Send(method, path, content, false);
		private async Task<string> Send(string method, string path, object content, bool hasResponse)
		{
			Stopwatch stopwatch = null;
			string requestJson = null;
			if (content != null)
				requestJson = JsonConvert.SerializeObject(content);

			if (_config.LogLevel >= LogSeverity.Verbose)
				stopwatch = Stopwatch.StartNew();
			
			string responseJson = await _engine.Send(method, path, requestJson, _cancelToken).ConfigureAwait(false);

#if TEST_RESPONSES
			if (!hasResponse && !string.IsNullOrEmpty(responseJson))
				throw new Exception("API check failed: Response is not empty.");
#endif

			if (_config.LogLevel >= LogSeverity.Verbose)
			{
				stopwatch.Stop();
				if (content != null && _config.LogLevel >= LogSeverity.Debug)
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

		private async Task<ResponseT> SendFile<ResponseT>(string method, string path, string filename, Stream stream)
			where ResponseT : class
		{
			string responseJson = await SendFile(method, path, filename, stream, true).ConfigureAwait(false);
			return DeserializeResponse<ResponseT>(responseJson);
		}
		private Task SendFile(string method, string path, string filename, Stream stream)
			=> SendFile(method, path, filename, stream, false);
		private async Task<string> SendFile(string method, string path, string filename, Stream stream, bool hasResponse)
		{
			Stopwatch stopwatch = null;

			if (_config.LogLevel >= LogSeverity.Verbose)
				stopwatch = Stopwatch.StartNew();
			
			string responseJson = await _engine.SendFile(method, path, filename, stream, _cancelToken).ConfigureAwait(false);

#if TEST_RESPONSES
			if (!hasResponse && !string.IsNullOrEmpty(responseJson))
				throw new Exception("API check failed: Response is not empty.");
#endif

			if (_config.LogLevel >= LogSeverity.Verbose)
			{
				stopwatch.Stop();
				if (_config.LogLevel >= LogSeverity.Debug)
					RaiseOnRequest(method, path, filename, stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond);
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
