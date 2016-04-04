using System.Threading;

namespace Discord.Net.Rest
{
    public delegate IRestEngine RestClientProvider(string baseUrl, CancellationToken cancelToken);
}
