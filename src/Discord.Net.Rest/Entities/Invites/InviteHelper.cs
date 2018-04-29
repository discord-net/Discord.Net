using System.Threading.Tasks;

namespace Discord.Rest
{
    internal static class InviteHelper
    {
        public static async Task DeleteAsync(IInvite invite, BaseDiscordClient client, 
            RequestOptions options)
        {
            await client.ApiClient.DeleteInviteAsync(invite.Code, options).ConfigureAwait(false);
        }
    }
}
