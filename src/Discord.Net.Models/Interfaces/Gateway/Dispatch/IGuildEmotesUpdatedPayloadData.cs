namespace Discord.Models;

public interface IGuildEmotesUpdatedPayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
    IEnumerable<IGuildEmoteModel> Emotes { get; }
}
