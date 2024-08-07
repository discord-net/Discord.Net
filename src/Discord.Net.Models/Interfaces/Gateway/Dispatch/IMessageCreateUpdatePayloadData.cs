namespace Discord.Models;

public interface IMessageCreateUpdatePayloadData : IGatewayPayloadData, IMessageModel
{
    ulong? GuildId { get; }
    IMemberModel? Member { get; }
    IEnumerable<IMentionedUser> Mentions { get; }
}
