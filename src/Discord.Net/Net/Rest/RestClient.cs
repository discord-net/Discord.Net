using Discord.API;
using Discord.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    public partial class RestClient
	{
        private struct RestResults
        {
            public string Response { get; set; }
            public double Milliseconds { get; set; }

            public RestResults(string response, double milliseconds)
            {
                Response = response;
                Milliseconds = milliseconds;
            }
        }

        public event EventHandler<RequestEventArgs> SendingRequest = delegate { };
        public event EventHandler<CompletedRequestEventArgs> SentRequest = delegate { };

        private void OnSendingRequest(IRestRequest request)
            => SendingRequest(this, new RequestEventArgs(request));
        private void OnSentRequest(IRestRequest request, object response, string responseJson, double milliseconds)
            => SentRequest(this, new CompletedRequestEventArgs(request, response, responseJson, milliseconds));

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

            if (Logger.Level >= LogSeverity.Verbose)
            {
                this.SentRequest += (s, e) =>
                {
                    string log = $"{e.Request.Method} {e.Request.Endpoint}: {e.Milliseconds} ms";
                    if (_config.LogLevel >= LogSeverity.Debug)
                    {
                        if (e.Request is IRestFileRequest)
                            log += $" [{(e.Request as IRestFileRequest).Filename}]";
                        else if (e.Response != null)
                        {
                            if (e.Request.IsPrivate)
                                log += $" [Hidden]";
                            else
                                log += $" {e.ResponseJson}";
                        }
                    }
                    Logger.Verbose(log);
                };
            }
        }

        public async Task<ResponseT> Send<ResponseT>(IRestRequest<ResponseT> request)
			where ResponseT : class
		{
            if (request == null) throw new ArgumentNullException(nameof(request));

            OnSendingRequest(request);
            var results = await Send(request, true).ConfigureAwait(false);
            var response = DeserializeResponse<ResponseT>(results.Response);
            OnSentRequest(request, response, results.Response, results.Milliseconds);

            return response;
        }
        public async Task Send(IRestRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            OnSendingRequest(request);
            var results = await Send(request, false).ConfigureAwait(false);
            OnSentRequest(request, null, null, results.Milliseconds);
        }

        public async Task<ResponseT> Send<ResponseT>(IRestFileRequest<ResponseT> request)
            where ResponseT : class
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            OnSendingRequest(request);
            var requestJson = JsonConvert.SerializeObject(request.Payload);
            var results = await SendFile(request, true).ConfigureAwait(false);
            var response = DeserializeResponse<ResponseT>(results.Response);
            OnSentRequest(request, response, results.Response, results.Milliseconds);

            return response;
        }
        public async Task Send(IRestFileRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            OnSendingRequest(request);
            var results = await SendFile(request, false).ConfigureAwait(false);
            OnSentRequest(request, null, null, results.Milliseconds);
        }

        private async Task<RestResults> Send(IRestRequest request, bool hasResponse)
        {
            object payload = request.Payload;
            string requestJson = null;
            if (payload != null)
				requestJson = JsonConvert.SerializeObject(payload);

            Stopwatch stopwatch = Stopwatch.StartNew();
            string responseJson = await _engine.Send(request.Method, request.Endpoint, requestJson, CancelToken).ConfigureAwait(false);
            stopwatch.Stop();

            double milliseconds = Math.Round((double)stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);            
            return new RestResults(responseJson, milliseconds);
		}
        
		private async Task<RestResults> SendFile(IRestFileRequest request, bool hasResponse)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();			
			string responseJson = await _engine.SendFile(request.Method, request.Endpoint, request.Filename, request.Stream, CancelToken).ConfigureAwait(false);
            stopwatch.Stop();

            double milliseconds = Math.Round((double)stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);
            return new RestResults(responseJson, milliseconds);
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
