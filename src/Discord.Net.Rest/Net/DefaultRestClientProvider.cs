using System;

namespace Discord.Net.Rest
{
    public static class DefaultRestClientProvider
    {
        public static readonly RestClientProvider Instance = Create();

        /// <exception cref="PlatformNotSupportedException">The default RestClientProvider is not supported on this platform.</exception>
        public static RestClientProvider Create(bool useProxy = false)
        {
            return url =>
            {
                try
                {
                    return new DefaultRestClient(url, useProxy);
                }
                catch (PlatformNotSupportedException ex)
                {
                    throw new PlatformNotSupportedException("The default RestClientProvider is not supported on this platform.", ex);
                }
            };
        }
    }
}
