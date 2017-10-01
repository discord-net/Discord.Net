using Discord.Commands;
using Moq;
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
            await service.AddModuleAsync(typeof(DummyModule)).ConfigureAwait(false);

            var contextMock = new Mock<ICommandContext>();
            var context = contextMock.Object;

            foreach (var _name in DummyModule.CommandNames)
            {
                var name = _name.Replace(' ', separatorChar);

                var result = service.Search(context, name);
                Assert.True(result.IsSuccess, result.ErrorReason);
            }
        }

        [Theory]
        [MemberData(nameof(CaseSensitivityTestData), MemberType = typeof(CommandServiceConfigTests))]
        public async Task CaseSensitivity(bool caseSensitive, bool upperCase)
        {
            var service = new CommandService(new CommandServiceConfig { CaseSensitiveCommands = caseSensitive });
            await service.AddModuleAsync(typeof(DummyModule)).ConfigureAwait(false);

            var contextMock = new Mock<ICommandContext>();
            var context = contextMock.Object;

            foreach (var _name in DummyModule.CommandNames)
            {
                var name = upperCase ? _name.ToUpper() : _name;

                var result = service.Search(context, name);
                if (caseSensitive && upperCase)
                    Assert.False(result.IsSuccess, $"Searching for `{name}` returned successfully");
                else
                    Assert.True(result.IsSuccess, result.ErrorReason);
            }
        }
    }
}
