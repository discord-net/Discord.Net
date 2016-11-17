using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Rest;
using System.Collections.ObjectModel;

namespace Discord.Tests.Framework
{
    public class MockRestClient : IRestClient
    {
        public MockRestClient(string baseUrl)
        {
            _requestHandler = new RequestHandler();
        }

        private Dictionary<string, string> _headers = new Dictionary<string, string>();
        public IReadOnlyDictionary<string, string> Headers =>
            new ReadOnlyDictionary<string, string>(_headers);
        private RequestHandler _requestHandler;

        public Task<Stream> SendAsync(string method, string endpoint, bool headerOnly = false) =>
            SendAsync(method, endpoint, "", headerOnly);

        public Task<Stream> SendAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams, bool headerOnly = false)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> SendAsync(string method, string endpoint, string json, bool headerOnly = false)
        {
            return Task.FromResult(_requestHandler.GetMock(method, endpoint, json, Headers));
        }

        public void SetCancelToken(CancellationToken cancelToken) { }

        public void SetHeader(string key, string value)
        {
            if (_headers.ContainsKey(key))
                _headers.Remove(key);
            _headers.Add(key, value);
        }
    }
}
