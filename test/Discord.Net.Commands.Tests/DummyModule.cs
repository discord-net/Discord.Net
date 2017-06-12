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
    }
}
