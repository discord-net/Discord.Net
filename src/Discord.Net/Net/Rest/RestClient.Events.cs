using System;

namespace Discord.Net.Rest
{
    internal sealed partial class RestClient
	{
		public class RequestEventArgs : EventArgs
		{
			public string Method { get; }
			public string Path { get; }
			public string Payload { get; }
			public double ElapsedMilliseconds { get; }
			public RequestEventArgs(string method, string path, string payload, double milliseconds)
			{
				Method = method;
				Path = path;
				Payload = payload;
				ElapsedMilliseconds = milliseconds;
            }
		}

		public event EventHandler<RequestEventArgs> OnRequest;
		private void RaiseOnRequest(string method, string path, string payload, double milliseconds)
		{
			if (OnRequest != null)
				OnRequest(this, new RequestEventArgs(method, path, payload, milliseconds));
		}
	}
}
