using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Discord.Net.Commands.Tests
{
    public partial class CommandServiceConfigTests
    {
        [Theory]
        [MemberData(nameof(SeparatorNodeTestData), MemberType = typeof(CommandServiceConfigTests))]
        public async Task CommandSeparators(CommandServiceConfig config, char separatorChar)
        {
            var service = new CommandService(config);
            var module = await service.AddModuleAsync(typeof(DummyModule)).ConfigureAwait(false);

            Assert.True(CommandsLoaded(service, separatorChar));

            var dummyContext = new DummyCommandContext();

            foreach (var _alias in DefaultAliases)
            {
                var alias = _alias.Replace(' ', separatorChar);

                var result = service.Search(dummyContext, alias);
                Assert.True(result.IsSuccess, result.ErrorReason);
            }
        }

        public bool CommandsLoaded(CommandService service, char separatorChar)
        {
            if (!service.Commands.Any())
                return false;

            var loadedAliases = service.Commands.SelectMany(x => x.Aliases);

            foreach (var alias in DefaultAliases)
            {
                if (!loadedAliases.Contains(alias.Replace(' ', separatorChar)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
