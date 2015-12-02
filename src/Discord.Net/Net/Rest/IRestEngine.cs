using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
	internal interface IRestEngine
	{
		void SetToken(string token);
		Task<string> Send(string method, string path, string json, CancellationToken cancelToken);
		Task<string> SendFile(string method, string path, string filename, Stream stream, CancellationToken cancelToken);
	}
}
