namespace Discord
{
	public sealed class Region
    {
        public string Id { get; }
        public string Name { get; }
        public string Hostname { get; }
        public int Port { get; }

        internal Region(string id, string name, string hostname, int port)
        {
            Id = id;
            Name = name;
            Hostname = hostname;
            Port = port;
        }
	}
}
