using Discord.API;
using Discord.Logging;
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
        private string _token;
        private JsonSerializerSettings _deserializeSettings;

        internal Logger Logger { get; }

        public CancellationToken CancelToken { get; set; }

        public string Token
        {
            get { return _token; }
            set
            {
                _token = value;
                _engine.SetToken(value);
            }
        }

        public RestClient(DiscordConfig config, string baseUrl, Logger logger)
		{
			_config = config;
            Logger = logger;

#if !DOTNET5_4
			_engine = new RestSharpEngine(config, baseUrl, logger);
#else
			_engine = new BuiltInEngine(config, baseUrl, logger);
#endif

            _deserializeSettings = new JsonSerializerSettings();
#if TEST_RESPONSES
            _deserializeSettings.CheckAdditionalContent = true;
            _deserializeSettings.MissingMemberHandling = MissingMemberHandling.Error;
#else
            _deserializeSettings.CheckAdditionalContent = false;
            _deserializeSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
#endif
        }

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
            if (Logger.Level >= LogSeverity.Verbose)
				stopwatch = Stopwatch.StartNew();

            string responseJson = await _engine.Send(method, path, requestJson, CancelToken).ConfigureAwait(false);

			if (Logger.Level >= LogSeverity.Verbose)
			{
				stopwatch.Stop();
                double milliseconds = Math.Round(stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond, 2);

                string log = $"{method} {path}: {milliseconds} ms";
                if (payload != null && _config.LogLevel >= LogSeverity.Debug)
				{
					if (isPrivate)
                        log += $" [Hidden]";
                    else
                        log += $" {requestJson}";
				}
                Logger.Verbose(log);
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
			if (Logger.Level >= LogSeverity.Verbose)
				stopwatch = Stopwatch.StartNew();
			
			string responseJson = await _engine.SendFile(method, path, filename, stream, CancelToken).ConfigureAwait(false);

			if (Logger.Level >= LogSeverity.Verbose)
			{
				stopwatch.Stop();
                double milliseconds = Math.Round(stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond, 2);

                string log = $"{method} {path}: {milliseconds} ms";
                if (_config.LogLevel >= LogSeverity.Debug && !isPrivate)
                    log += $" {filename}";
                Logger.Verbose(log);
            }

			return responseJson;
		}

		private T DeserializeResponse<T>(string json)
		{
#if TEST_RESPONSES
			if (string.IsNullOrEmpty(json))
				throw new Exception("API check failed: Response is empty.");
#endif
            return JsonConvert.DeserializeObject<T>(json, _deserializeSettings);
		}
	}
}
