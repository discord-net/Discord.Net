using Discord.ComponentDesigner.LanguageServer;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "{Timestamp:HH:mm:ss} | {Level} - [{SourceContext}]: {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

var server = await LanguageServer.From(options => options
    .WithInput(Console.OpenStandardInput())
    .WithOutput(Console.OpenStandardOutput())
    .ConfigureLogging(x => x
        .AddSerilog(Log.Logger)
        .AddLanguageProtocolLogging()
        .SetMinimumLevel(LogLevel.Debug)
    )
    .AddHandler<DocumentHandler>()
    .OnInitialize(((languageServer, request, token) =>
    {
        request.
    }))
);
