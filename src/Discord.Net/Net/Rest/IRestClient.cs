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
        void SetCancelToken(CancellationToken cancelToken);

        Task<Stream> SendAsync(string method, string endpoint, bool headerOnly = false);
        Task<Stream> SendAsync(string method, string endpoint, string json, bool headerOnly = false);
        Task<Stream> SendAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams, bool headerOnly = false);
    }
}
