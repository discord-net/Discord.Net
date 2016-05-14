using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.Net.Rest;
using System.Threading;
using System.IO;

namespace Discord.Tests.Rest
{
    class TestRestClient : IRestClient
    {
        public static Dictionary<string, string> Headers = new Dictionary<string, string>();

        public TestRestClient(string baseUrl, CancellationToken cancelToken)
        {

        }

        Task<Stream> IRestClient.Send(string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams)
        {
            throw new NotImplementedException("method only used for SendFile, not concerned with that yet.");
        }

        Task<Stream> IRestClient.Send(string method, string endpoint, string json)
        {
            return Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes(EndpointHandler.Instance.HandleMessage(method, endpoint, json))));
        }

        void IRestClient.SetHeader(string key, string value)
        {
            if (Headers.ContainsKey(key))
            {
                Headers.Remove(key);
            }
            Headers.Add(key, value);
            Console.WriteLine($"[Header Set]: {key} -> {value}");
        }
    }
}
