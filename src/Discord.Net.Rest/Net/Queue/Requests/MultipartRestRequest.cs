using Discord.Net.Rest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    public class MultipartRestRequest : RestRequest
    {
        public IReadOnlyDictionary<string, object> MultipartParams { get; }
        
        public MultipartRestRequest(IRestClient client, string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams, RequestOptions options)
            : base(client, method, endpoint, options)
        {
            MultipartParams = multipartParams;
        }

        public override async Task<RestResponse> SendAsync()
        {
            return await Client.SendAsync(Method, Endpoint, MultipartParams, Options.CancelToken, Options.HeaderOnly).ConfigureAwait(false);
        }
    }
}
