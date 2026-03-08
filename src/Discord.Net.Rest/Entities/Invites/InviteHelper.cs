using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Rest;

internal static class InviteHelper
{
    public static Task DeleteAsync(IInvite invite, BaseDiscordClient client, RequestOptions options)
        => client.ApiClient.DeleteInviteAsync(invite.Code, options);

    public static async Task<RestInviteMetadata> GetInviteAsync(BaseDiscordClient client, string inviteId, RequestOptions options, ulong? scheduledEventId = null)
    {
        var model = await client.ApiClient.GetInviteAsync(inviteId, options, scheduledEventId).ConfigureAwait(false);

        return model is not null
            ? RestInviteMetadata.Create(client, null, null, model)
            : null;
    }

    public static async Task<IReadOnlyCollection<ulong>> GetInviteTargetUsersAsync(BaseDiscordClient client, string inviteCode, RequestOptions options)
    {
        return [.. await client.ApiClient.GetInviteTargetUsersAsync(inviteCode).ConfigureAwait(false)];
    }

    public static async Task<TargetUsersJobStatus> GetTargetUsersJobStatusAsync(BaseDiscordClient client, string inviteId, RequestOptions options)
    {
        var model = await client.ApiClient.GetInviteTargetUsersJobStatusAsync(inviteId, options).ConfigureAwait(false);

        return new TargetUsersJobStatus(model.Status, model.TotalUsers, model.ProcessedUsers, model.CreatedAt, model.CompletedAt, model.ErrorMessage);
    }

    public static Task ModifyInviteTargetUsersAsync(BaseDiscordClient client, string inviteId, IEnumerable<ulong> userIds, RequestOptions options)
        => client.ApiClient.ModifyInviteTargetUsersAsync(inviteId, new (userIds), options);
}
