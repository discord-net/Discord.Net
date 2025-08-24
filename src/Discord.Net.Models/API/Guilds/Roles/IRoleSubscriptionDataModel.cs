using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IRoleSubscriptionDataModel : IModel
{
    Snowflake RoleSubscriptionListingId { get; }
    
    string TierName { get; }
    
    int TotalMonthsSubscribed { get; }
    
    bool IsRenewal { get; }
}