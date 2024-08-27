namespace Discord.Models;

public interface IGuildEmotesUpdatedPayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
    IEnumerable<ICustomEmoteModel> Emotes { get; }
}
