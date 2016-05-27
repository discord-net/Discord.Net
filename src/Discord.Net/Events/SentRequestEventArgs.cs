using System;

namespace Discord.Net.Rest
{
    public class SentRequestEventArgs : EventArgs
    {
        public string Method { get; }
        public string Endpoint { get; }
        public int ResponseLength { get; }
        public double Milliseconds { get; }

        public SentRequestEventArgs(string method, string endpoint, int responseLength, double milliseconds)
        {
            Method = method;
            Endpoint = endpoint;
            ResponseLength = responseLength;
            Milliseconds = milliseconds;
        }
    }
}
