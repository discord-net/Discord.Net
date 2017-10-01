using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Commands.Tests
{
    [Group("debug")]
    class DummyModule : ModuleBase
    {
        [Command("ping")]
        public Task TestCommandAsync(string param)
        {
            return Task.Delay(0);
        }

        [Command("pong")]
        public Task AnotherCommandAsync(int param)
        {
            return Task.Delay(0);
        }

        //NOTE: do not add this to CommandNames: it is intentional for this command to not be loaded!
        [Command("doesNotLoad")]
        private Task DoesNotLoadAsync()
        {
            return Task.Delay(0);
        }

        public static IEnumerable<string> CommandNames => _commandNames;
        private static readonly IEnumerable<string> _commandNames = new List<string>
        {
            "debug ping",
            "debug pong"
        };
    }
}
