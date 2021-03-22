using Discord.Net.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Rest.Net
{
    public class HttpClientFactoryRestClientProvider
    {
        public readonly RestClientProvider Instance;

        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientFactoryRestClientProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            Instance = url => new HttpClientFactoryRestClient(url, _httpClientFactory.CreateClient("HttpClientFactoryRestClientProvider"), useProxy: false);
        }



    }
}
