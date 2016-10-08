using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    public interface IRestClient
    {
        void SetHeader(string key, string value);
        void SetCancelToken(CancellationToken cancelToken);

        Task<Stream> SendAsync(string method, string endpoint, RequestOptions options);
        Task<Stream> SendAsync(string method, string endpoint, string json, RequestOptions options);
        Task<Stream> SendAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams, RequestOptions options);
    }
}
