namespace Discord
{
	public class Region
    {
        public string Id { get; }
        public string Name { get; }
        public string Hostname { get; }
        public int Port { get; }
        public bool Vip { get; }

        internal Region(string id, string name, string hostname, int port, bool vip)
        {
            Id = id;
            Name = name;
            Hostname = hostname;
            Port = port;
            Vip = vip;
        }
    }
}
