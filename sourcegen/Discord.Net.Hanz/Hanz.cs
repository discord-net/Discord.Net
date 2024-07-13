using Discord.Net.Hanz.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Reflection;

namespace Discord.Net.Hanz;

[Generator]
public sealed class Hanz : IIncrementalGenerator
{
    internal static ILogger Logger = NullLogger.Instance;

    public record struct LoggingOptions(string FilePath, LogLevel Level);

    private readonly MethodInfo _registerTaskMethod = typeof(Hanz).GetMethods(BindingFlags.Static | BindingFlags.Public).First(x => x.Name.StartsWith("RegisterTask"));
    private readonly MethodInfo _registerCombineTaskMethod = typeof(Hanz).GetMethods(BindingFlags.Static | BindingFlags.Public).First(x => x.Name.StartsWith("RegisterCombineTask"));

    public static void RegisterTask<T>(IncrementalGeneratorInitializationContext context, IGenerationTask<T> task)
        where T : class
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: task.IsValid,
            transform: task.GetTargetForGeneration
        );

        context.RegisterSourceOutput(provider, task.Execute);

        Logger.Log($"Registered {task.GetType().Name} task");
    }

    public static void RegisterCombineTask<T>(IncrementalGeneratorInitializationContext context, IGenerationCombineTask<T> task)
        where T : class
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: task.IsValid,
            transform: task.GetTargetForGeneration
        ).Collect();

        context.RegisterSourceOutput(provider, task.Execute);

        Logger.Log($"Registered {task.GetType().Name} task");
    }

    private static bool IsGenerationTask(Type type)
    {
        return type.IsGenericType && (
            type.GetGenericTypeDefinition() == typeof(IGenerationTask<>) ||
            type.GetGenericTypeDefinition() == typeof(IGenerationCombineTask<>)
        );
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var options = context.AnalyzerConfigOptionsProvider.Select((options, _) => GetLoggingOptions(options));

        SetupLogger(context, options);

        var generationTasks = typeof(Hanz).Assembly.GetTypes()
            .Where(x => x
                .GetInterfaces()
                .Any(IsGenerationTask)
            );

        foreach (var task in generationTasks)
        {
            var generationInterface = task.GetInterfaces().FirstOrDefault(IsGenerationTask);

            if (generationInterface is null) continue;

            if(generationInterface.GetGenericTypeDefinition() == typeof(IGenerationTask<>))
                _registerTaskMethod.MakeGenericMethod(generationInterface.GenericTypeArguments[0])
                    .Invoke(null, [context, Activator.CreateInstance(task)]);
            else if(generationInterface.GetGenericTypeDefinition() == typeof(IGenerationCombineTask<>))
                _registerCombineTaskMethod.MakeGenericMethod(generationInterface.GenericTypeArguments[0])
                    .Invoke(null, [context, Activator.CreateInstance(task)]);
        }
    }

    private void SetupLogger(
        IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<LoggingOptions?> optionsProvider)
    {
        var logging = optionsProvider
            .Select((options, _) =>
            {
                Logger = options is null
                    ? NullLogger.Instance
                    : new Logger(options.Value.Level,
                        options.Value.FilePath);

                return 0;
            })
            .SelectMany((_, _) => ImmutableArray<int>.Empty);

        context.RegisterSourceOutput(logging, static (_, _) => { });
    }

    private static LoggingOptions? GetLoggingOptions(AnalyzerConfigOptionsProvider options)
    {
        if (!options.GlobalOptions.TryGetValue("build_property.HanzLogFilePath",
                out var logFilePath))
            return null;

        if (string.IsNullOrWhiteSpace(logFilePath))
            return null;

        logFilePath = logFilePath.Trim();

        if (!options.GlobalOptions.TryGetValue("build_property.HanzLogLevel",
                out var logLevelValue)
            || !Enum.TryParse(logLevelValue, true, out LogLevel logLevel))
        {
            logLevel = LogLevel.Information;
        }

        return new LoggingOptions(logFilePath, logLevel);
    }
}
