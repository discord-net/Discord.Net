using Discord.API;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
	public sealed partial class RestClient
	{
		private readonly DiscordConfig _config;
		private readonly IRestEngine _engine;
		private CancellationToken _cancelToken;

		public RestClient(DiscordConfig config, Logger logger)
		{
			_config = config;
#if !DOTNET5_4
			_engine = new RestSharpEngine(config, logger, DiscordConfig.ClientAPIUrl);
#else
			//_engine = new BuiltInRestEngine(config, logger);
#endif
        }

        public void SetToken(string token) => _engine.SetToken(token);
        public void SetCancelToken(CancellationToken token) => _cancelToken = token;

        public async Task<ResponseT> Send<ResponseT>(IRestRequest<ResponseT> request)
			where ResponseT : class
		{
            if (request == null) throw new ArgumentNullException(nameof(request));
            
            string responseJson = await Send(request, true).ConfigureAwait(false);
            return DeserializeResponse<ResponseT>(responseJson);
		}
        public Task Send(IRestRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            
            return Send(request, false);
        }

        public async Task<ResponseT> Send<ResponseT>(IRestFileRequest<ResponseT> request)
            where ResponseT : class
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            string requestJson = JsonConvert.SerializeObject(request.Payload);
            string responseJson = await SendFile(request, true).ConfigureAwait(false);
            return DeserializeResponse<ResponseT>(responseJson);
        }
        public Task Send(IRestFileRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            
            return SendFile(request, false);
        }

        private async Task<string> Send(IRestRequest request, bool hasResponse)
        {
            var method = request.Method;
            var path = request.Endpoint;
            object payload = request.Payload;
            var isPrivate = request.IsPrivate;

            string requestJson = null;
            if (payload != null)
				requestJson = JsonConvert.SerializeObject(payload);

            Stopwatch stopwatch = null;
            if (_config.LogLevel >= LogSeverity.Verbose)
				stopwatch = Stopwatch.StartNew();

            string responseJson = await _engine.Send(method, path, requestJson, _cancelToken).ConfigureAwait(false);

			if (_config.LogLevel >= LogSeverity.Verbose)
			{
				stopwatch.Stop();
				if (payload != null && _config.LogLevel >= LogSeverity.Debug)
				{
					if (isPrivate)
                        RaiseOnRequest(method, path, "[Hidden]", stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond);
					else
						RaiseOnRequest(method, path, requestJson, stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond);
				}
				else
					RaiseOnRequest(method, path, null, stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond);
			}

			return responseJson;
		}
        
		private async Task<string> SendFile(IRestFileRequest request, bool hasResponse)
		{
            var method = request.Method;
            var path = request.Endpoint;
            var filename = request.Filename;
            var stream = request.Stream;
            var isPrivate = request.IsPrivate;

			Stopwatch stopwatch = null;
			if (_config.LogLevel >= LogSeverity.Verbose)
				stopwatch = Stopwatch.StartNew();
			
			string responseJson = await _engine.SendFile(method, path, filename, stream, _cancelToken).ConfigureAwait(false);

			if (_config.LogLevel >= LogSeverity.Verbose)
			{
				stopwatch.Stop();
				if (_config.LogLevel >= LogSeverity.Debug && !isPrivate)
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
	}
}
