using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    //TODO: Add docstrings
    public interface IRestClient
    {
        /// <summary> Sets a header to be used in REST requests. </summary>
        void SetHeader(string key, string value);
        /// <summary> Sets the global cancellation token for any requests made by this instance. </summary>
        void SetCancelToken(CancellationToken cancelToken);

        /// <summary> Sends a request with no body to the given endpoint. </summary>
        Task<Stream> SendAsync(string method, string endpoint, bool headerOnly = false);
        /// <summary> Sends a request with a body to the given endpoint. </summary>
        Task<Stream> SendAsync(string method, string endpoint, string json, bool headerOnly = false);
        /// <summary> Sends a multipart request with the given parameters to the given endpoint. </summary>
        Task<Stream> SendAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams, bool headerOnly = false);
    }
}
