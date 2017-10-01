using Discord.Commands;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Net.Commands.Tests
{
    partial class CommandServiceConfigTests
    {
        public static IEnumerable<object[]> SeparatorNodeTestData => _separatorNodeTestData;
        private static readonly IEnumerable<object[]> _separatorNodeTestData = new List<CommandServiceConfig>
        {
            new CommandServiceConfig{ SeparatorChar = ' '},
            new CommandServiceConfig{ SeparatorChar = '_'},
            new CommandServiceConfig{ SeparatorChar = '.'},
            new CommandServiceConfig{ SeparatorChar = '\u200b'},
            new CommandServiceConfig{ SeparatorChar = '"'}
        }.Select(x => new object[] { x, x.SeparatorChar });

        public static IEnumerable<object[]> CaseSensitivityTestData => _caseSensitivityTestData;
        private static readonly IEnumerable<object[]> _caseSensitivityTestData = new object[][]
        {
            new object[]{  true, false },
            new object[]{  true,  true },
            new object[]{ false,  true },
            new object[]{ false, false }
        };
    }
}
