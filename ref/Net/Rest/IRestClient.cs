using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    public interface IRestClient
	{
        event EventHandler<RequestEventArgs> SendingRequest;
        event EventHandler<CompletedRequestEventArgs> SentRequest;

        CancellationToken CancelToken { get; }
        string Token { get; }

        Task<ResponseT> Send<ResponseT>(IRestRequest<ResponseT> request)
            where ResponseT : class;
        Task Send(IRestRequest request);

        Task<ResponseT> Send<ResponseT>(IRestFileRequest<ResponseT> request)
            where ResponseT : class;
        Task Send(IRestFileRequest request);
	}
}
