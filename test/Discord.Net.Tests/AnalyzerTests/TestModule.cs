using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Test
{
    public class TestModule : ModuleBase<ICommandContext>
    {
        [Command("test"), RequireContext(ContextType.Group)]
        public Task TestCmd() => ReplyAsync(Context.Guild.Name);
    }
}
