using System;
using System.IO;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    public interface IRestEngine : IDisposable
    {
        void SetHeader(string key, string value);
        
        Task<Stream> Send(IRestRequest request);
        Task<Stream> Send(IRestFileRequest request);
    }
}
