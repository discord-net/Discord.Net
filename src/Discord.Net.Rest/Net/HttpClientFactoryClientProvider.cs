using Discord.Net.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Rest.Net
{
    public class HttpFactoryRestClientProvider
    {
        public readonly RestClientProvider Instance;

        private readonly IHttpClientFactory _httpClientFactory;

        public HttpFactoryRestClientProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            Instance = url => new HttpClientFactoryClient(url, _httpClientFactory.CreateClient("HttpFactoryRestClientProvider"), useProxy: false);
        }



    }
}
