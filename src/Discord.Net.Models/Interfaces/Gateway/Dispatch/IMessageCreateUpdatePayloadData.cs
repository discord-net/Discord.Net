namespace Discord.Models;

public interface IMessageCreateUpdatePayloadData : IGatewayPayloadData, IMessageModel
{
    ulong? GuildId { get; }
    IPartialMemberModel? Member { get; }
    IEnumerable<IMentionedUser> Mentions { get; }
}
