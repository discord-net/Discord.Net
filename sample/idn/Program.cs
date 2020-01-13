using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Discord;

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
            "Discord.Socket",
            "idn"
        };

        static async Task Main(string[] args)
        {
            var token = File.ReadAllText("token.ignore");
            var client = IDiscordClient.Create(token);
            // client.start

            var options = ScriptOptions.Default
                .AddReferences(GetAssemblies().ToArray())
                .AddImports(Imports);

            var globals = new ScriptGlobals
            {
                Client = client,
            };

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (input == "quit")
                {
                    break;
                }

                object eval;
                try
                {
                    eval = await CSharpScript.EvaluateAsync(input, options, globals);
                }
                catch (Exception e)
                {
                    eval = e;
                }
                Console.WriteLine(Inspector.Inspect(eval));
            }

            // client.Stop
            client.Dispose();
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
            public IDiscordClient Client { get; set; }
        }
    }
}
