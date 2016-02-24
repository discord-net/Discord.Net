using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    public abstract partial class RestClient
	{
        public event EventHandler<RequestEventArgs> SendingRequest = delegate { };
        public event EventHandler<CompletedRequestEventArgs> SentRequest = delegate { };

        public CancellationToken CancelToken { get; set; }
        public string Token { get; set; }
        
        public Task<ResponseT> Send<ResponseT>(IRestRequest<ResponseT> request)
			where ResponseT : class 
            => null;
        public Task Send(IRestRequest request) => null;

        public Task<ResponseT> Send<ResponseT>(IRestFileRequest<ResponseT> request)
            where ResponseT : class 
            => null;
        public Task Send(IRestFileRequest request)  => null;
	}
}
