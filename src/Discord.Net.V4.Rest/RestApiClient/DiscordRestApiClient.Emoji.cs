using Discord.Models;

namespace Discord.Rest;

public partial class DiscordRestApiClient
{
    public virtual Task<IReadOnlyCollection<IEmojiModel>> ListGuildEmojisAsync(ulong guildId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IEmojiModel> GetGuildEmojiAsync(ulong guildId, ulong emojiId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IEmojiModel> CreateGuildEmoji(string name, Image image, ICollection<ulong> roleIds, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IEmojiModel> ModifyGuildEmoji(string? name = null, ICollection<ulong>? roleIds = null, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task DeleteGuildEmojiAsync(ulong guildId, ulong emojiId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IApplicationRoleConnectionModel?> GetUserApplicationRoleConnectionAsync(ulong applicationId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IApplicationRoleConnectionModel> UpdateUserApplicationRoleConnectionAsync(string? platformName = null, string? platformUsername = null, IDictionary<string, string>? metadata = null, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();
}
