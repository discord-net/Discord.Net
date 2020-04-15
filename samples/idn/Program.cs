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
            var client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Debug });
            var logQueue = new ConcurrentQueue<LogMessage>();
            var logCancelToken = new CancellationTokenSource();

            client.Log += msg =>
            {
                logQueue.Enqueue(msg);
                return Task.CompletedTask;
            };

            var logTask = Task.Run(async () =>
            {
                var fs = new FileStream("idn.log", FileMode.Append);
                //var f = File.Open("idn.log", FileMode.Append);
                StringBuilder logStringBuilder = new StringBuilder(200);
                string logString = "";

                byte[] helloBytes = Encoding.UTF8.GetBytes($"### new log session: {DateTime.Now} ###\n\n");
                await fs.WriteAsync(helloBytes);

                while (!logCancelToken.IsCancellationRequested)
                {
                    if (logQueue.TryDequeue(out var msg))
                    {
                        _ = msg.ToString(builder: logStringBuilder);
                        logStringBuilder.AppendLine();
                        logString = logStringBuilder.ToString();

                        Debug.Write(logString, "DNET");
                        await fs.WriteAsync(Encoding.UTF8.GetBytes(logString), logCancelToken.Token);
                    }
                    await fs.FlushAsync();
                    await Task.Delay(100, logCancelToken.Token);
                }

                byte[] goodbyeBytes = Encoding.UTF8.GetBytes($"#!! end log session: {DateTime.Now} !!#\n\n\n");
                await fs.WriteAsync(goodbyeBytes);
                await fs.DisposeAsync();
            });

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

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

                if (input == "quit!")
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

            await client.StopAsync();
            client.Dispose();
            logCancelToken.Cancel();
            try
            { await logTask; }
            finally { Console.WriteLine("goodbye!"); }
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
        }
    }
}
