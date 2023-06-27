using Discord.Models;

namespace Discord;

public interface IRestApiProvider : IAsyncDisposable, IDisposable
{
    Task LoginAsync(TokenType tokenType, string token, bool validateToken, CancellationToken? cancellationToken = null);

    Task LogoutAsync(CancellationToken? cancellationToken = null);

    #region User

    Task<IUserModel> GetCurrentUserAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IUserModel?> GetUserAsync(ulong userId, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IUserModel> ModifyCurrentUserAsync(string username, Image? image = null, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IReadOnlyCollection<IGuildModel>> GetCurrentUserGuildsAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IUserModel?> GetCurrentUserGuildMemberAsync(ulong guildId, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task LeaveGuildAsync(ulong guildId, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IDMChannelModel> CreateDMChannelAsync(ulong recipientId, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IGroupDMChannelModel> CreateGroupDMChannelAsync(ICollection<string> accessTokens, IDictionary<ulong, string> nicknames,
        CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IReadOnlyCollection<IUserConnectionModel>> GetUserConnectionsAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IApplicationRoleConnectionModel?> GetUSerApplicationRoleConnection(ulong applicationId, CancellationToken? cancellationToken = null, RequestOptions? options = null);

    Task<IApplicationRoleConnectionModel> UpdateUserApplicationRoleConnection(string? platformName = null, string? platformUsername = null, IDictionary<string, string>? metadata = null,
        CancellationToken? cancellationToken = null, RequestOptions? options = null);

    #endregion

}
