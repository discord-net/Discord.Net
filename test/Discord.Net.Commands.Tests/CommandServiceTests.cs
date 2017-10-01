using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Discord.Net.Commands.Tests
{
    class CommandServiceTests
    {
        [Fact]
        public async Task CommandsLoad()
        {
            var service = new CommandService();

            var module = await service.AddModuleAsync(typeof(DummyModule)).ConfigureAwait(false);
            Assert.NotNull(module);

            var commandAliases = module.Commands.SelectMany(x => x.Aliases);

            foreach (var name in DummyModule.CommandNames)
            {
                Assert.True(commandAliases.Contains(name), $"The loaded module did not contain the command {name}");
            }
        }

        [Fact]
        public async Task MultipleLoadsThrows()
        {
            var service = new CommandService();

            var module = await service.AddModuleAsync(typeof(DummyModule)).ConfigureAwait(false);
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddModuleAsync(typeof(DummyModule)))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task InvalidTypeThrows()
        {
            var service = new CommandService();
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddModuleAsync(typeof(CommandServiceTests)))
                .ConfigureAwait(false);
        }
    }
}
