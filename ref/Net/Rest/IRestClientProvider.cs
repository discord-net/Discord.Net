using System.Collections.Generic;
using System.Threading;

namespace Discord.Net.Rest
{
    public interface IRestClientProvider
    {
        IRestClient Create(string baseUrl, CancellationToken cancelToken);
    }
}
