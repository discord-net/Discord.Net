using System.Threading.Tasks;

namespace Discord.Commands
{
    public abstract class ModuleBase
    {
        public CommandContext Context { get; internal set; }

        protected virtual async Task ReplyAsync(string message, bool isTTS = false, RequestOptions options = null)
        {
            await Context.Channel.SendMessageAsync(message, isTTS, options).ConfigureAwait(false);
        }
    }
}
