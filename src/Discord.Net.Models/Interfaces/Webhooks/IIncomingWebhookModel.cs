namespace Discord.Models;

[ModelEquality]
public partial interface IIncomingWebhookModel : IWebhookModel
{
    ulong GuildId { get; }
    ulong ChannelId { get; }
    string Url { get; }
}