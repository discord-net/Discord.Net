namespace Audio.Dependencies
{
    internal abstract class Dependency
    {
        public abstract string Name { get; }
        public abstract string DownloadUrl { get; }

        public abstract bool IsInstalled();
        public abstract Task DownloadAsync(HttpClient httpClient);
    }
}
