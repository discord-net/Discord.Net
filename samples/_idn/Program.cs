using Discord;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Idn
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
            int presenceUpdates = 0;

            client.Log += msg =>
            {
                logQueue.Enqueue(msg);
                return Task.CompletedTask;
            };
            Console.CancelKeyPress += (_ev, _s) =>
            {
                logCancelToken.Cancel();
            };

            var logTask = Task.Run(async () =>
            {
                var fs = new FileStream("idn.log", FileMode.Append);
                var logStringBuilder = new StringBuilder(200);
                string logString = "";

                byte[] helloBytes = Encoding.UTF8.GetBytes($"### new log session: {DateTime.Now} ###\n\n");
                await fs.WriteAsync(helloBytes);

                while (!logCancelToken.IsCancellationRequested)
                {
                    if (logQueue.TryDequeue(out var msg))
                    {
                        if (msg.Message?.IndexOf("PRESENCE_UPDATE)") > 0)
                        {
                            presenceUpdates++;
                            continue;
                        }

                        _ = msg.ToString(builder: logStringBuilder);
                        logStringBuilder.AppendLine();
                        logString = logStringBuilder.ToString();

                        Debug.Write(logString, "DNET");
                        await fs.WriteAsync(Encoding.UTF8.GetBytes(logString));
                    }
                    await fs.FlushAsync();
                    try
                    {
                        await Task.Delay(100, logCancelToken.Token);
                    }
                    finally { }
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
            public int PUCount { get; set; }
        }
    }
}
