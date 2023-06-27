using Discord.Models;

namespace Discord.Rest;

public partial class DiscordRestApiClient
{
    public Task<IUserModel> GetCurrentUserAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public Task<IUserModel> GetUserAsync(ulong userId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public Task<IUserModel> ModifyCurrentUserAsync(string username, Image? image = null, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public Task<IReadOnlyCollection<IGuildModel>> GetCurrentUserGuildsAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public Task<IUserModel> GetCurrentUserGuildMemberAsync(ulong guildId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public Task LeaveGuildAsync(ulong guildId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public Task<IDMChannelModel> CreateDMChannelAsync(ulong recipientId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public Task<IGroupDMChannelModel> CreateGroupDMChannelAsync(ICollection<string> accessTokens, IDictionary<ulong, string> nicknames, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public Task<IReadOnlyCollection<IUserConnectionModel>> GetUserConnectionsAsync(CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public Task<IApplicationRoleConnectionModel?> GetUSerApplicationRoleConnection(ulong applicationId, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();

    public Task<IApplicationRoleConnectionModel> UpdateUserApplicationRoleConnection(string? platformName = null, string? platformUsername = null, IDictionary<string, string>? metadata = null, CancellationToken? cancellationToken = null, RequestOptions? options = null) => throw new NotImplementedException();
}
