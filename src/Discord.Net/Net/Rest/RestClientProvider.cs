using System.Threading;

namespace Discord.Net.Rest
{
    public delegate IRestClient RestClientProvider(string baseUrl, CancellationToken cancelToken);
}
