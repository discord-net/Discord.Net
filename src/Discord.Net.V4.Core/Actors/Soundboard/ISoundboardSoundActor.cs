using Discord.Rest;

namespace Discord;

public partial interface ISoundboardSoundActor :
    IActor<ulong, ISoundboardSound>
{
    Task SendAsync(
        EntityOrId<ulong, IChannelActor> channel,
        ulong? soundGuildId = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        return Client.RestApiClient.ExecuteAsync(
            Routes.SendSoundboardSound(
                channel.Id,
                Id,
                this is IGuildSoundboardSoundActor guildSound 
                    ? guildSound.Guild.Id 
                    : soundGuildId
            ),
            options,
            token
        );
    }
}