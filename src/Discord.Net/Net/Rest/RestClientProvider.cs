namespace Discord.Net.Rest
{
    /// <summary> A delegate for creating a user-defined implementation of <see cref="IRestClient"/> </summary>
    public delegate IRestClient RestClientProvider(string baseUrl);
}
