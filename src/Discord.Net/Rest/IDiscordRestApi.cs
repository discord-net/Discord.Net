using System;
using System.Threading.Tasks;
using Refit;
using Discord.Models;

namespace Discord.Rest
{
    public interface IDiscordRestApi
    {
        // --- /applications

        // --- /channels

        // --- /gateway
        [Get("/gateway/bot")]
        Task<GatewayInfo> GetGatewayInfoAsync();
        [Get("/gateway/bot")]
        Task<GatewayInfo> GetBotGatewayInfoAsync();

        // --- /guilds

        // --- /invites

        // --- /oauth2

        // --- /store

        // --- /users

        // --- /voice

        // --- /webhooks
    }
}
