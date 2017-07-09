using System.Threading.Tasks;

namespace Discord
{
    public static class UserExtensions
    {
        public static async Task<IUserMessage> SendMessageAsync(this IUser user, 
            string text, 
            bool isTTS = false,
            Embed embed = null, 
            RequestOptions options = null)
        {
            return await (await user.GetOrCreateDMChannelAsync().ConfigureAwait(false)).SendMessageAsync(text, isTTS, embed, options).ConfigureAwait(false);
        }
    }
}
