using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    //TODO: Add docstrings
    public interface IRestClient
    {
        void SetHeader(string key, string value);

        Task<Stream> Send(string method, string endpoint, CancellationToken cancelToken, string json = null, bool headerOnly = false);
        Task<Stream> Send(string method, string endpoint, CancellationToken cancelToken, IReadOnlyDictionary<string, object> multipartParams, bool headerOnly = false);
    }
}
