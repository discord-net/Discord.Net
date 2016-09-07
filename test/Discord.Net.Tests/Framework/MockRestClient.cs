using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Rest;

namespace Discord.Tests.Framework
{
    public class MockRestClient : IRestClient
    {
        public MockRestClient(string baseUrl)
        { }

        Task<Stream> IRestClient.SendAsync(string method, string endpoint, bool headerOnly)
        {
            throw new NotImplementedException();
        }

        Task<Stream> IRestClient.SendAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams, bool headerOnly)
        {
            throw new NotImplementedException();
        }

        Task<Stream> IRestClient.SendAsync(string method, string endpoint, string json, bool headerOnly)
        {
            throw new NotImplementedException();
        }

        void IRestClient.SetCancelToken(CancellationToken cancelToken)
        {
            throw new NotImplementedException();
        }

        void IRestClient.SetHeader(string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}
