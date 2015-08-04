using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Helpers
{
	internal class HttpOptions
	{
		public string UserAgent, Token;
		public CookieContainer Cookies;

		public HttpOptions(string userAgent = null)
		{
			UserAgent = userAgent ?? "DiscordAPI";
			Cookies = new CookieContainer(1);
		}
	}

	internal static class Http
	{
		private static readonly RequestCachePolicy _cachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

		internal static async Task<ResponseT> Get<ResponseT>(string path, object data, HttpOptions options)
			where ResponseT : class
		{
			string requestJson = JsonConvert.SerializeObject(data);
			string responseJson = await SendRequest("GET", path, requestJson, options, true);
			return JsonConvert.DeserializeObject<ResponseT>(responseJson);
		}
		internal static async Task<ResponseT> Get<ResponseT>(string path, HttpOptions options)
			where ResponseT : class
		{
			string responseJson = await SendRequest("GET", path, null, options, true);
			return JsonConvert.DeserializeObject<ResponseT>(responseJson);
		}

		internal static async Task<ResponseT> Post<ResponseT>(string path, object data, HttpOptions options)
			where ResponseT : class
		{
			string requestJson = JsonConvert.SerializeObject(data);
			string responseJson = await SendRequest("POST", path, requestJson, options, true);
			return JsonConvert.DeserializeObject<ResponseT>(responseJson);
		}
		internal static Task Post(string path, object data, HttpOptions options)
		{
			string requestJson = JsonConvert.SerializeObject(data);
			return SendRequest("POST", path, requestJson, options, false);
		}
		internal static async Task<ResponseT> Post<ResponseT>(string path, HttpOptions options)
			where ResponseT : class
		{
			string responseJson = await SendRequest("POST", path, null, options, true);
			return JsonConvert.DeserializeObject<ResponseT>(responseJson);
		}
		internal static Task Post(string path, HttpOptions options)
		{
			return SendRequest("POST", path, null, options, false);
		}

		internal static Task Delete(string path, HttpOptions options)
		{
			return SendRequest("DELETE", path, null, options, false);
		}

		private static async Task<string> SendRequest(string method, string path, string data, HttpOptions options, bool hasResponse)
		{
			options = options ?? new HttpOptions();

			//Create Request
			HttpWebRequest request = WebRequest.CreateHttp(path);
			request.Accept = "*/*";
			request.Headers[HttpRequestHeader.AcceptLanguage] = "en-US;q=0.8";
			request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
			request.CachePolicy = _cachePolicy;
			request.CookieContainer = options.Cookies;
			request.Method = method;
			request.UserAgent = options.UserAgent;

			//Add Payload
			if (data != null)
			{
				byte[] buffer = Encoding.UTF8.GetBytes(data);
				using (var payload = await request.GetRequestStreamAsync())
					payload.Write(buffer, 0, buffer.Length);
				request.ContentType = "application/json";
			}

			//Get Response
			using (var response = (HttpWebResponse)(await request.GetResponseAsync()))
			{
				if (hasResponse)
				{
					using (var stream = response.GetResponseStream())
					using (var reader = new BinaryReader(stream))
					using (var largeBuffer = new MemoryStream())
					{
						//Read the response in small chunks and add them to a larger buffer.
						//ContentLength isn't always provided, so this is safer.
						int bytesRead = 0;
						byte[] smallBuffer = new byte[4096];
						while ((bytesRead = reader.Read(smallBuffer, 0, smallBuffer.Length)) > 0)
							largeBuffer.Write(smallBuffer, 0, bytesRead);

						//Do we need to decompress?
						if (!string.IsNullOrEmpty(response.ContentEncoding))
						{
							largeBuffer.Position = 0;
							using (var decoder = GetDecoder(response.ContentEncoding, largeBuffer))
							using (var decodedStream = new MemoryStream())
							{
								decoder.CopyTo(decodedStream);
								return Encoding.UTF8.GetString(decodedStream.GetBuffer(), 0, (int)decodedStream.Length);
							}
						}
						else
							return Encoding.UTF8.GetString(largeBuffer.GetBuffer(), 0, (int)largeBuffer.Length);
					}
				}
				else
					return null;
			}
		}

		private static Stream GetDecoder(string contentEncoding, MemoryStream encodedStream)
		{
			switch (contentEncoding)
			{
				case "gzip":
					return new GZipStream(encodedStream, CompressionMode.Decompress, true);
				case "deflate":
					return new DeflateStream(encodedStream, CompressionMode.Decompress, true);
				default:
					throw new ArgumentOutOfRangeException("Unknown encoding: " + contentEncoding);
			}
		}
	}
}
