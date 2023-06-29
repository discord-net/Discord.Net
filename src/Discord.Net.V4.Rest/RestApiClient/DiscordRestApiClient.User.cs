using Discord.Models;

namespace Discord.Rest;

public partial class DiscordRestApiClient
{
    public virtual Task<IUserModel> GetCurrentUserAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IUserModel> GetUserAsync(ulong userId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IUserModel> ModifyCurrentUserAsync(string username, Image? image = null, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IReadOnlyCollection<IGuildModel>> GetCurrentUserGuildsAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IUserModel> GetCurrentUserGuildMemberAsync(ulong guildId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task LeaveGuildAsync(ulong guildId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IDMChannelModel> CreateDMChannelAsync(ulong recipientId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IGroupDMChannelModel> CreateGroupDMChannelAsync(ICollection<string> accessTokens, IDictionary<ulong, string> nicknames, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IReadOnlyCollection<IUserConnectionModel>> GetUserConnectionsAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IApplicationRoleConnectionModel?> GetUSerApplicationRoleConnection(ulong applicationId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IApplicationRoleConnectionModel> UpdateUserApplicationRoleConnection(string? platformName = null, string? platformUsername = null, IDictionary<string, string>? metadata = null, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();
}
