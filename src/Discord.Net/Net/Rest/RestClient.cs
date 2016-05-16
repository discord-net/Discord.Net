using Discord.API;
using Discord.ETF;
using Discord.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    public abstract partial class RestClient
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

        private bool OnSendingRequest(IRestRequest request)
        {
            var eventArgs = new RequestEventArgs(request);
            SendingRequest(this, eventArgs);
            return !eventArgs.Cancel;
        }
        private void OnSentRequest(IRestRequest request, object response, string responseJson, double milliseconds)
            => SentRequest(this, new CompletedRequestEventArgs(request, response, responseJson, milliseconds));

        private readonly DiscordConfig _config;
		private readonly IRestEngine _engine;
        private readonly ILogger _logger;
        private string _token;

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

        protected RestClient(DiscordConfig config, string baseUrl, ILogger logger = null)
		{
			_config = config;
            _logger = logger;

#if !NETSTANDARD1_3
            _engine = new RestSharpEngine(config, baseUrl, logger);
#else
			_engine = new BuiltInEngine(config, baseUrl, logger);
#endif

            if (logger != null && logger.Level >= LogSeverity.Verbose)
                SentRequest += (s, e) => _logger.Verbose($"{e.Request.Method} {e.Request.Endpoint}: {e.Milliseconds} ms");
        }

        public async Task<ResponseT> Send<ResponseT>(IRestRequest<ResponseT> request)
			where ResponseT : class
		{
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!OnSendingRequest(request)) throw new OperationCanceledException();
            var results = await Send(request, true).ConfigureAwait(false);
            var response = Deserialize<ResponseT>(results.Response);
            OnSentRequest(request, response, results.Response, results.Milliseconds);

            return response;
        }
        public async Task Send(IRestRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!OnSendingRequest(request)) throw new OperationCanceledException();
            var results = await Send(request, false).ConfigureAwait(false);
            OnSentRequest(request, null, null, results.Milliseconds);
        }

        public async Task<ResponseT> Send<ResponseT>(IRestFileRequest<ResponseT> request)
            where ResponseT : class
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!OnSendingRequest(request)) throw new OperationCanceledException();
            var results = await SendFile(request, true).ConfigureAwait(false);
            var response = Deserialize<ResponseT>(results.Response);
            OnSentRequest(request, response, results.Response, results.Milliseconds);

            return response;
        }
        public async Task Send(IRestFileRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!OnSendingRequest(request)) throw new OperationCanceledException();
            var results = await SendFile(request, false).ConfigureAwait(false);
            OnSentRequest(request, null, null, results.Milliseconds);
        }

        private async Task<RestResults> Send(IRestRequest request, bool hasResponse)
        {
            object payload = request.Payload;
            string requestJson = null;
            if (payload != null)
				requestJson = Serialize(payload);

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

        protected abstract string Serialize<T>(T obj);
        protected abstract T Deserialize<T>(string json);
	}
}
