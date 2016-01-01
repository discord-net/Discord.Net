using System;

namespace Discord.Net.Rest
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
}
