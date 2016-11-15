using System.Threading.Tasks;

namespace Discord.Commands
{
    public abstract class ModuleBase
    {
        public CommandContext Context { get; internal set; }

        protected virtual async Task<IUserMessage> ReplyAsync(string message, bool isTTS = false, EmbedBuilder embed = null, RequestOptions options = null)
        {
            return await Context.Channel.SendMessageAsync(message, isTTS, embed, options).ConfigureAwait(false);
        }
    }
}
