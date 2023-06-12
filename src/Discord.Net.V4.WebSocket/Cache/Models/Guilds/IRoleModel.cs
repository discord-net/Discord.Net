using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IRoleModel : IEntityModel<ulong>
    {
        string Name { get; }
        int Color { get; }
        bool IsHoisted { get; }
        string? Icon { get; }
        string? UnicodeEmoji { get; }
        int Position { get; }
        ulong Permissions { get; }
        bool IsManaged { get; }
        bool IsMentionable { get; }

        // tags
        ulong? BotId { get; }
        ulong? IntegrationId { get; }
        bool IsPremiumSubscriberRole { get; }
        ulong? SubscriptionListingId { get; }
        bool AvailableForPurchase { get; }
        bool IsGuildConnection { get; }
    }
}
