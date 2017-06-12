using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.Net.Commands.Tests
{
    class DummyCommandContext : ICommandContext
    {
        public IDiscordClient Client { get; set; }

        public IGuild Guild { get; set; }

        public IMessageChannel Channel { get; set; }

        public IUser User { get; set; }

        public IUserMessage Message { get; set; }
    }
}
