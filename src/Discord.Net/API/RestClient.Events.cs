using System;
using System.Net.Http;

namespace Discord.API
{
    internal partial class RestClient
	{
		public class RequestEventArgs : EventArgs
		{
			public HttpMethod Method { get; }
			public string Path { get; }
			public string Payload { get; }
			public double ElapsedMilliseconds { get; }
			public RequestEventArgs(HttpMethod method, string path, string payload, double milliseconds)
			{
				Method = method;
				Path = path;
				Payload = payload;
				ElapsedMilliseconds = milliseconds;
            }
		}

		public event EventHandler<RequestEventArgs> OnRequest;
		protected void RaiseOnRequest(HttpMethod method, string path, string payload, double milliseconds)
		{
			if (OnRequest != null)
				OnRequest(this, new RequestEventArgs(method, path, payload, milliseconds));
		}
	}
}
