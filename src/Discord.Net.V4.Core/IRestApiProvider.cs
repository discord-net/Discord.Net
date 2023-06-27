using Discord.Models;

namespace Discord;

public interface IRestApiProvider : IAsyncDisposable, IDisposable
{
    Task LoginAsync(TokenType tokenType, string token, bool validateToken, CancellationToken? cancellationToken = null);

    Task LogoutAsync(CancellationToken? cancellationToken = null);

    #region Emoji

    Task<IReadOnlyCollection<IEmojiModel>> ListGuildEmojisAsync(ulong guildId, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IEmojiModel> GetGuildEmojiAsync(ulong guildId, ulong emojiId, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IEmojiModel> CreateGuildEmoji(string name, Image image, ICollection<ulong> roleIds, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IEmojiModel> ModifyGuildEmoji(string? name = null, ICollection<ulong>? roleIds = null, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task DeleteGuildEmojiAsync(ulong guildId, ulong emojiId, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    #endregion

    #region User

    Task<IUserModel> GetCurrentUserAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IUserModel> GetUserAsync(ulong userId, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IUserModel> ModifyCurrentUserAsync(string username, Image? image = null, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IReadOnlyCollection<IGuildModel>> GetCurrentUserGuildsAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IUserModel> GetCurrentUserGuildMemberAsync(ulong guildId, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task LeaveGuildAsync(ulong guildId, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IDMChannelModel> CreateDMChannelAsync(ulong recipientId, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IGroupDMChannelModel> CreateGroupDMChannelAsync(ICollection<string> accessTokens, IDictionary<ulong, string> nicknames,
        CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IReadOnlyCollection<IUserConnectionModel>> GetUserConnectionsAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IApplicationRoleConnectionModel?> GetUserApplicationRoleConnectionAsync(ulong applicationId, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IApplicationRoleConnectionModel> UpdateUserApplicationRoleConnectionAsync(string? platformName = null, string? platformUsername = null, IDictionary<string, string>? metadata = null,
        CancellationToken? cancellationToken = null, RequestOptions? options = null);

    #endregion

    #region Voice

    Task<IReadOnlyCollection<IVoiceRegion>> ListVoiceRegionsAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null);

    #endregion


}
