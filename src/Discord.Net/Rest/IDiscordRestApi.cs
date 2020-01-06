using System.Threading.Tasks;
using Refit;
using Discord.Rest.Models;

namespace Discord.Rest
{
    public interface IDiscordRestApi
    {
        [Get("/gateway/bot")]
        Task<GatewayInfo> GetGatewayInfoAsync();
    }
}
