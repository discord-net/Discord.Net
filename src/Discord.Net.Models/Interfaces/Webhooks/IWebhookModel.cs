namespace Discord.Models;

[ModelEquality]
public partial interface IWebhookModel : IEntityModel<ulong>
{
    int Type { get; }
    ulong? UserId { get; }
    string? Name { get; }
    string? Avatar { get; }
    ulong? ApplicationId { get; }
}
