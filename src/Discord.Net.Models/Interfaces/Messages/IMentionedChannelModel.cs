namespace Discord.Models;

[ModelEquality]
public partial interface IMentionedChannelModel : IModel
{
    ulong Id { get; }
    ulong GuildId { get; }
    int Type { get; }
    string Name { get; }
}