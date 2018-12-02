using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Xunit;

namespace Discord
{
    public sealed class TypeReaderTests
    {
        [Fact]
        public async Task TestNamedArgumentReader()
        {
            var commands = new CommandService();
            var module = await commands.AddModuleAsync<TestModule>(null);

            Assert.NotNull(module);
            Assert.NotEmpty(module.Commands);

            var cmd = module.Commands[0];
            Assert.NotNull(cmd);
            Assert.NotEmpty(cmd.Parameters);

            var param = cmd.Parameters[0];
            Assert.NotNull(param);
            Assert.True(param.IsRemainder);

            var result = await param.ParseAsync(null, "bar: hello foo: 42");
            Assert.True(result.IsSuccess);

            var m = result.BestMatch as ArgumentType;
            Assert.NotNull(m);
            Assert.Equal(expected: 42, actual: m.Foo);
            Assert.Equal(expected: "hello", actual: m.Bar);
        }

        [Fact]
        public async Task TestQuotedArgumentValue()
        {
            var commands = new CommandService();
            var module = await commands.AddModuleAsync<TestModule>(null);

            Assert.NotNull(module);
            Assert.NotEmpty(module.Commands);

            var cmd = module.Commands[0];
            Assert.NotNull(cmd);
            Assert.NotEmpty(cmd.Parameters);

            var param = cmd.Parameters[0];
            Assert.NotNull(param);
            Assert.True(param.IsRemainder);

            var result = await param.ParseAsync(null, "foo: 42 bar: 《hello》");
            Assert.True(result.IsSuccess);

            var m = result.BestMatch as ArgumentType;
            Assert.NotNull(m);
            Assert.Equal(expected: 42, actual: m.Foo);
            Assert.Equal(expected: "hello", actual: m.Bar);
        }

        [Fact]
        public async Task TestNonPatternInput()
        {
            var commands = new CommandService();
            var module = await commands.AddModuleAsync<TestModule>(null);

            Assert.NotNull(module);
            Assert.NotEmpty(module.Commands);

            var cmd = module.Commands[0];
            Assert.NotNull(cmd);
            Assert.NotEmpty(cmd.Parameters);

            var param = cmd.Parameters[0];
            Assert.NotNull(param);
            Assert.True(param.IsRemainder);

            var result = await param.ParseAsync(null, "foobar");
            Assert.False(result.IsSuccess);
            Assert.Equal(expected: CommandError.Exception, actual: result.Error);
        }

        [Fact]
        public async Task TestMultiple()
        {
            var commands = new CommandService();
            var module = await commands.AddModuleAsync<TestModule>(null);

            Assert.NotNull(module);
            Assert.NotEmpty(module.Commands);

            var cmd = module.Commands[0];
            Assert.NotNull(cmd);
            Assert.NotEmpty(cmd.Parameters);

            var param = cmd.Parameters[0];
            Assert.NotNull(param);
            Assert.True(param.IsRemainder);

            var result = await param.ParseAsync(null, "manyints: \"1, 2, 3, 4, 5, 6, 7\"");
            Assert.True(result.IsSuccess);

            var m = result.BestMatch as ArgumentType;
            Assert.NotNull(m);
            Assert.Equal(expected: new int[] { 1, 2, 3, 4, 5, 6, 7 }, actual: m.ManyInts);
        }
    }

    [NamedArgumentType]
    public sealed class ArgumentType
    {
        public int Foo { get; set; }

        [OverrideTypeReader(typeof(CustomTypeReader))]
        public string Bar { get; set; }

        public IEnumerable<int> ManyInts { get; set; }
    }

    public sealed class CustomTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
            => Task.FromResult(TypeReaderResult.FromSuccess(input));
    }

    public sealed class TestModule : ModuleBase
    {
        [Command("test")]
        public Task TestCommand(ArgumentType arg) => Task.Delay(0);
    }
}
