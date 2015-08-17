using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Helpers
{
	internal class HttpOptions
	{
		public readonly string UserAgent;
		public string Token;

		public HttpOptions(string userAgent)
		{
			UserAgent = userAgent;
		}
	}

	internal static class Http
	{
#if DEBUG
		private const bool _isDebug = true;
#else
		private const bool _isDebug = false;
#endif
		
		internal static Task<ResponseT> Get<ResponseT>(string path, HttpOptions options)
			where ResponseT : class
			=> Send<ResponseT>("GET", path, null, options);
		internal static Task<string> Get(string path, HttpOptions options)
			=> Send("GET", path, null, options);
		
		internal static Task<ResponseT> Post<ResponseT>(string path, object data, HttpOptions options)
			where ResponseT : class
			=> Send<ResponseT>("POST", path, data, options);
		internal static Task<string> Post(string path, object data, HttpOptions options)
			=> Send("POST", path, data, options);
		internal static Task<ResponseT> Post<ResponseT>(string path, HttpOptions options)
			where ResponseT : class
			=> Send<ResponseT>("POST", path, null, options);
		internal static Task<string> Post(string path, HttpOptions options)
			=> Send("POST", path, null, options);
		
		internal static Task<ResponseT> Put<ResponseT>(string path, object data, HttpOptions options)
			where ResponseT : class
			=> Send<ResponseT>("PUT", path, data, options);
		internal static Task<string> Put(string path, object data, HttpOptions options)
			=> Send("PUT", path, data, options);
		internal static Task<ResponseT> Put<ResponseT>(string path, HttpOptions options)
			where ResponseT : class
			=> Send<ResponseT>("PUT", path, null, options);
		internal static Task<string> Put(string path, HttpOptions options)
			=> Send("PUT", path, null, options);

		internal static Task<ResponseT> Patch<ResponseT>(string path, object data, HttpOptions options)
			where ResponseT : class
			=> Send<ResponseT>("PATCH", path, data, options);
		internal static Task<string> Patch(string path, object data, HttpOptions options)
			=> Send("PATCH", path, data, options);
		internal static Task<ResponseT> Patch<ResponseT>(string path, HttpOptions options)
			where ResponseT : class
			=> Send<ResponseT>("PATCH", path, null, options);
		internal static Task<string> Patch(string path, HttpOptions options)
			=> Send("PATCH", path, null, options);

		internal static Task<ResponseT> Delete<ResponseT>(string path, object data, HttpOptions options)
			where ResponseT : class
			=> Send<ResponseT>("DELETE", path, data, options);
		internal static Task<string> Delete(string path, object data, HttpOptions options)
			=> Send("DELETE", path, data, options);
		internal static Task<ResponseT> Delete<ResponseT>(string path, HttpOptions options)
			where ResponseT : class
			=> Send<ResponseT>("DELETE", path, null, options);
		internal static Task<string> Delete(string path, HttpOptions options)
			=> Send("DELETE", path, null, options);

		internal static async Task<ResponseT> Send<ResponseT>(string method, string path, object data, HttpOptions options)
			where ResponseT : class
		{
			string requestJson = data != null ? JsonConvert.SerializeObject(data) : null;
			string responseJson = await SendRequest(method, path, requestJson, options, true);
			var response = JsonConvert.DeserializeObject<ResponseT>(responseJson);
#if DEBUG
			CheckResponse(responseJson, response);
#endif
			return response;
		}
		internal static async Task<string> Send(string method, string path, object data, HttpOptions options)
		{
			string requestJson = data != null ? JsonConvert.SerializeObject(data) : null;
			string responseJson = await SendRequest(method, path, requestJson, options, _isDebug);
#if DEBUG
			CheckEmptyResponse(responseJson);
#endif
			return responseJson;
		}

		private static async Task<string> SendRequest(string method, string path, string data, HttpOptions options, bool hasResponse)
		{
			//Create Request
			HttpWebRequest request = WebRequest.CreateHttp(path);
			request.Accept = "*/*";
			request.Method = method;
			request.Proxy = null;
			request.Headers[HttpRequestHeader.AcceptLanguage] = "en-US;q=0.8";
			request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
			request.Headers[HttpRequestHeader.UserAgent] = options.UserAgent;
			request.Headers[HttpRequestHeader.Authorization] = options.Token;
			//request.UserAgent = options.UserAgent;

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
						string encoding = response.Headers[HttpResponseHeader.ContentEncoding];
						if (!string.IsNullOrEmpty(encoding))
						{
							largeBuffer.Position = 0;
							using (var decoder = GetDecoder(encoding, largeBuffer))
							using (var decodedStream = new MemoryStream())
							{
								decoder.CopyTo(decodedStream);
#if !DOTNET
								return Encoding.UTF8.GetString(decodedStream.GetBuffer(), 0, (int)decodedStream.Length);
#else
								ArraySegment<byte> buffer;
								if (!decodedStream.TryGetBuffer(out buffer))
									throw new InvalidOperationException("Failed to get response buffer.");
								return Encoding.UTF8.GetString(buffer.Array, buffer.Offset, (int)decodedStream.Length);
#endif
							}
						}
						else
						{
#if !DOTNET
							return Encoding.UTF8.GetString(largeBuffer.GetBuffer(), 0, (int)largeBuffer.Length);
#else
							ArraySegment<byte> buffer;
							if (!largeBuffer.TryGetBuffer(out buffer))
								throw new InvalidOperationException("Failed to get response buffer.");
							return Encoding.UTF8.GetString(buffer.Array, buffer.Offset, (int)largeBuffer.Length);
#endif
						}
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

#if DEBUG
		private static void CheckResponse<T>(string json, T obj)
		{
			/*JToken token = JToken.Parse(json);
			JToken token2 = JToken.FromObject(obj);
			if (!JToken.DeepEquals(token, token2))
				throw new Exception("API check failed: Objects do not match.");*/
		}

		private static void CheckEmptyResponse(string json)
		{
			if (!string.IsNullOrEmpty(json))
				throw new Exception("API check failed: Response is not empty.");
		}
#endif
	}
}
