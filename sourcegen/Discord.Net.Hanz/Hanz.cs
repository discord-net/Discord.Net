using Discord.Net.Hanz.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;

namespace Discord.Net.Hanz;

[Generator]
public sealed class Hanz : IIncrementalGenerator
{
    public static LoggingOptions LoggerOptions { get; private set; } = new(LogLevel.Information);

    private static Logger _rootLogger = new(LogLevel.Information, Path.Combine(Logger.LogDirectory, "root.log"));

    public record struct LoggingOptions(LogLevel Level);

    private readonly MethodInfo _registerTaskMethod = typeof(Hanz).GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(x => x.Name.StartsWith("RegisterTask"));

    private readonly MethodInfo _registerCombineTaskMethod = typeof(Hanz)
        .GetMethods(BindingFlags.Static | BindingFlags.Public).First(x => x.Name.StartsWith("RegisterCombineTask"));

    private readonly struct TransformWrapper<T>(Logger logger, T? value) where T : class, IEquatable<T>
    {
        public readonly Logger Logger = logger;
        public readonly T? Value = value;

        public bool Equals(TransformWrapper<T> other) =>
            Value is null
                ? other.Value is null
                : other.Value is not null && Value.Equals(other.Value);

        public readonly override int GetHashCode() =>
            Value is null ? 0 : EqualityComparer<T>.Default.GetHashCode(Value);
    }

    public static void RegisterTask<T>(IncrementalGeneratorInitializationContext context, IGenerationTask<T> task)
        where T : class, IEquatable<T>
    {
        var assemblyLoggers = new ConcurrentDictionary<string, Logger>();

        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: task.IsValid,
            transform: (syntaxContext, token) =>
            {
                try
                {
                    var logger = assemblyLoggers.GetOrAdd(
                        syntaxContext.SemanticModel.Compilation.Assembly.Name,
                        assembly => Logger.CreateSemanticRunForTask(assembly, task.GetType().Name));

                    return new TransformWrapper<T>(logger,
                        task.GetTargetForGeneration(syntaxContext, logger.GetSubLogger("transform").WithCleanLogFile(),
                            token));
                }
                catch (Exception ex)
                {
                    _rootLogger.Log(LogLevel.Error, $"Failed to run generation task {task}: {ex}");
                    throw;
                }
            }
        );

        context.RegisterSourceOutput(provider, (context, wrapper) =>
        {
            try
            {
                //wrapper.Logger.Clean();

                task.Execute(context, wrapper.Value, wrapper.Logger.GetSubLogger("execute").WithCleanLogFile());
            }
            catch (Exception ex)
            {
                _rootLogger.Log(LogLevel.Error, $"Failed to run generation task {task}: {ex}");
                throw;
            }
        });

        _rootLogger.Log($"Registered {task.GetType().Name} task");
    }

    public static void RegisterCombineTask<T>(IncrementalGeneratorInitializationContext context,
        IGenerationCombineTask<T> task)
        where T : class, IEquatable<T>
    {
        var logger = Logger.CreateForTask(task.GetType().Name);
        var transformLogger = Logger.CreateForTask(task.GetType().Name);

        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: task.IsValid,
            transform: (syntaxContext, token) =>
            {
                try
                {
                    return task.GetTargetForGeneration(
                        syntaxContext,
                        transformLogger
                            .WithSemanticContext(syntaxContext.SemanticModel)
                            .GetSubLogger("transform")
                            .WithCleanLogFile(),
                        token
                    );
                }
                catch (Exception ex)
                {
                    _rootLogger.Log(LogLevel.Error, $"Failed to run generation task {task}: {ex}");
                    throw;
                }
            }
        ).Collect();

        context.RegisterSourceOutput(provider, (productionContext, array) =>
        {
            try
            {
                logger.Clean();
                task.Execute(productionContext, array, logger);
            }
            catch (Exception ex)
            {
                _rootLogger.Log(LogLevel.Error, $"Failed to run generation task {task}: {ex}");
                throw;
            }
        });

        _rootLogger.Log($"Registered {task.GetType().Name} task");
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
        try
        {
            _rootLogger.DeleteLogFile();

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

                if (generationInterface.GetGenericTypeDefinition() == typeof(IGenerationTask<>))
                    _registerTaskMethod.MakeGenericMethod(generationInterface.GenericTypeArguments[0])
                        .Invoke(null, [context, Activator.CreateInstance(task)]);
                else if (generationInterface.GetGenericTypeDefinition() == typeof(IGenerationCombineTask<>))
                    _registerCombineTaskMethod.MakeGenericMethod(generationInterface.GenericTypeArguments[0])
                        .Invoke(null, [context, Activator.CreateInstance(task)]);
            }

            var jsonTaskLogger = Logger.CreateForTask("JsonModels");

            context.RegisterSourceOutput(
                context.SyntaxProvider
                    .CreateSyntaxProvider(
                        (node, _) => node is ClassDeclarationSyntax,
                        (context, _) => JsonModels.GetTarget(context)
                    )
                    .Collect()
                    .Combine(context.CompilationProvider)
                    .Combine(context.AnalyzerConfigOptionsProvider.Select((options, _) =>
                    {
                        TryGetProjectName(options, out var projectName);
                        options.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNameSpace);
                        return (ProjectName: projectName, RootNameSpace: rootNameSpace);
                    })),
                (productionContext, data) =>
                {
                    var logger = jsonTaskLogger;
                    if (data.Right.ProjectName is not null)
                        logger = logger.GetSubLogger(data.Right.ProjectName).WithCleanLogFile();

                    JsonModels.Execute(productionContext, data, logger);
                }
            );
        }
        catch (Exception x)
        {
            SelfLog.Write($"Failed to initialize {x}");
        }
    }

    private void SetupLogger(
        IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<LoggingOptions?> optionsProvider)
    {
        var logging = optionsProvider
            .Select((options, _) =>
            {
                if (options is not null) LoggerOptions = options.Value;
                return 0;
            })
            .SelectMany((_, _) => ImmutableArray<int>.Empty);

        context.RegisterSourceOutput(logging, static (_, _) => { });
    }

    private static LoggingOptions? GetLoggingOptions(AnalyzerConfigOptionsProvider options)
    {
        if (!options.GlobalOptions.TryGetValue("build_property.HanzLogLevel",
                out var logLevelValue)
            || !Enum.TryParse(logLevelValue, true, out LogLevel logLevel))
        {
            logLevel = LogLevel.Information;
        }

        if (TryGetProjectName(options, out var projectName) && projectName is not null)
        {
            _rootLogger = new Logger(logLevel,
                Path.Combine(Logger.LogDirectory, projectName, "global.log"));
            _rootLogger.DeleteLogFile();
        }

        return new LoggingOptions(logLevel);
    }

    private static bool TryGetProjectName(AnalyzerConfigOptionsProvider options, out string? project)
    {
        project = null;

        if (options.GlobalOptions.TryGetValue("build_property.projectdir", out var dir))
        {
            var split = dir.Split(Path.DirectorySeparatorChar);

            if (split.Length >= 2 && split[split.Length - 2].Contains("Discord"))
            {
                project = split[split.Length - 2];
            }
        }

        return project is not null;
    }
}
