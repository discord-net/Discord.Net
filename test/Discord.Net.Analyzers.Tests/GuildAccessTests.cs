using Discord.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using TestHelper;
using Xunit;

namespace Discord
{
    public partial class AnalyserTests
    {
        public class GuildAccessTests : DiagnosticVerifier
        {
            [Fact]
            public void VerifyDiagnosticWhenLackingRequireContext()
            {
                string source = @"using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Test
{
    public class TestModule : ModuleBase<ICommandContext>
    {
        [Command(""test"")]
        public Task TestCmd() => ReplyAsync(Context.Guild.Name);
    }
}";
                var expected = new DiagnosticResult()
                {
                    Id = "DNET0001",
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", line: 10, column: 45) },
                    Message = "Command method 'TestCmd' is accessing 'Context.Guild' but is not restricted to Guild contexts.",
                    Severity = DiagnosticSeverity.Warning
                };
                VerifyCSharpDiagnostic(source, expected);
            }

            [Fact]
            public void VerifyDiagnosticWhenWrongRequireContext()
            {
                string source = @"using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Test
{
    public class TestModule : ModuleBase<ICommandContext>
    {
        [Command(""test""), RequireContext(ContextType.Group)]
        public Task TestCmd() => ReplyAsync(Context.Guild.Name);
    }
}";
                var expected = new DiagnosticResult()
                {
                    Id = "DNET0001",
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", line: 10, column: 45) },
                    Message = "Command method 'TestCmd' is accessing 'Context.Guild' but is not restricted to Guild contexts.",
                    Severity = DiagnosticSeverity.Warning
                };
                VerifyCSharpDiagnostic(source, expected);
            }

            [Fact]
            public void VerifyNoDiagnosticWhenRequireContextOnMethod()
            {
                string source = @"using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Test
{
    public class TestModule : ModuleBase<ICommandContext>
    {
        [Command(""test""), RequireContext(ContextType.Guild)]
        public Task TestCmd() => ReplyAsync(Context.Guild.Name);
    }
}";

                VerifyCSharpDiagnostic(source, Array.Empty<DiagnosticResult>());
            }

            [Fact]
            public void VerifyNoDiagnosticWhenRequireContextOnClass()
            {
                string source = @"using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Test
{
    [RequireContext(ContextType.Guild)]
    public class TestModule : ModuleBase<ICommandContext>
    {
        [Command(""test"")]
        public Task TestCmd() => ReplyAsync(Context.Guild.Name);
    }
}";

                VerifyCSharpDiagnostic(source, Array.Empty<DiagnosticResult>());
            }

            protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
                => new GuildAccessAnalyzer();
        }
    }
}
