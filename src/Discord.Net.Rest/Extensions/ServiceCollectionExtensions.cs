using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Discord.Net.Rest;
using Discord.Rest.Net;

namespace Discord.Rest.Extensions
{
    /// <summary>
    /// Here you will find all the extensions to be able to add a Discord Client to an IServiceCollection 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedDiscordRestClient(this IServiceCollection services, bool useProxy = false)
        {
            services.AddHttpClient("HttpClientFactoryRestClientProvider")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    UseCookies = false,
                    UseProxy = useProxy,
                });



            services.AddTransient<HttpClientFactoryRestClientProvider>(provider => new HttpClientFactoryRestClientProvider(provider.GetRequiredService<IHttpClientFactory>()));
            services.AddScoped<DiscordRestClient>(provider =>
            {
                var config = new DiscordRestConfig
                {
                    RestClientProvider = provider.GetRequiredService<HttpClientFactoryRestClientProvider>().Instance
                };
                return new DiscordRestClient(config);
            });

            return services;
        }

        public static IServiceCollection AddTransientDiscordRestClient(this IServiceCollection services, bool useProxy = false) //where should we put this useProxy options, I haven't fully understood where the original code takes this from.
        {
            services.AddHttpClient("HttpClientFactoryRestClientProvider")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    UseCookies = false,
                    UseProxy = useProxy,
                });



            services.AddTransient<HttpClientFactoryRestClientProvider>(provider => new HttpClientFactoryRestClientProvider(provider.GetRequiredService<IHttpClientFactory>()));
            services.AddTransient<DiscordRestClient>(provider =>
            {
                var config = new DiscordRestConfig
                {
                    RestClientProvider = provider.GetRequiredService<HttpClientFactoryRestClientProvider>().Instance
                };
                return new DiscordRestClient(config);
            });

            return services;
        }
    }
}
