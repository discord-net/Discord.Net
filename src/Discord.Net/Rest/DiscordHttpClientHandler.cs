using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal sealed class DiscordHttpClientHandler : HttpClientHandler
    {
        private readonly AuthenticationHeaderValue _token;

        public DiscordHttpClientHandler(AuthenticationHeaderValue token)
        {
            _token = token;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = _token;
            return base.SendAsync(request, cancellationToken);
        }
    }
}
