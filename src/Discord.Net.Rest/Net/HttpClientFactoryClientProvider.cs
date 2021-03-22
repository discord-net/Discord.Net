using Discord.Net.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Rest.Net
{
    /// <summary>
    /// This RestClientProvider creates the IRestClient used when using an IHttpClientFactory.
    /// </summary>
    public class HttpClientFactoryRestClientProvider
    {
        public readonly RestClientProvider Instance;

        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Constructs the HttpClientFactoryRestClientProvider, the idea is to use this with DI so that the parameters gets fed with the right HttpClientFactory.
        /// </summary>
        /// <param name="httpClientFactory">This is the IHttpClientFactory used. It should be fed through DI.</param>
        public HttpClientFactoryRestClientProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            Instance = url => new HttpClientFactoryRestClient(url, _httpClientFactory.CreateClient("HttpClientFactoryRestClientProvider"), useProxy: false);
        }



    }
}
