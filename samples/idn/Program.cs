using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Discord;
using Discord.WebSocket;
using System.Collections.Concurrent;
using System.Threading;
using System.Text;
using System.Diagnostics;

namespace idn
{
    public class Program
    {
        public static readonly string[] Imports =
        {
            "System",
            "System.Collections.Generic",
            "System.Linq",
            "System.Threading.Tasks",
            "System.Diagnostics",
            "System.IO",
            "Discord",
            "Discord.Rest",
            "Discord.WebSocket",
            "idn"
        };

        static async Task Main(string[] args)
        {
            var token = File.ReadAllText("token.ignore");
            var client = new DiscordSocketClient(new DiscordSocketConfig { });
            var logCancelToken = new CancellationTokenSource();
            int presenceUpdates = 0;
            
            Console.CancelKeyPress += (_ev, _s) =>
            {
                logCancelToken.Cancel();
            };

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            var options = ScriptOptions.Default
                .AddReferences(GetAssemblies().ToArray())
                .AddImports(Imports);

            var globals = new ScriptGlobals
            {
                Client = client,
                PUCount = -1,
            };

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (input == "quit!")
                {
                    break;
                }

                object eval;
                try
                {
                    globals.PUCount = presenceUpdates;
                    eval = await CSharpScript.EvaluateAsync(input, options, globals);
                }
                catch (Exception e)
                {
                    eval = e;
                }
                Console.WriteLine(Inspector.Inspect(eval));
            }

            await client.StopAsync();
            client.Dispose();
            logCancelToken.Cancel();

            await Task.Delay(-1, logCancelToken.Token);
            Console.WriteLine("goodbye!");
        }

        static IEnumerable<Assembly> GetAssemblies()
        {
            var Assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
            foreach (var a in Assemblies)
            {
                var asm = Assembly.Load(a);
                yield return asm;
            }
            yield return Assembly.GetEntryAssembly();
        }

        public class ScriptGlobals
        {
            public DiscordSocketClient Client { get; set; }
            public int PUCount { get; set; }
        }
    }
}
