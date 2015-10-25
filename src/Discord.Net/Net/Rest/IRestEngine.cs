using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
	internal interface IRestEngine
	{
		void SetToken(string token);
		Task<string> Send(HttpMethod method, string path, string json, CancellationToken cancelToken);
		Task<string> SendFile(HttpMethod method, string path, string filePath, CancellationToken cancelToken);
	}
}
