using Discord.Net.Hanz.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Discord.Net.Hanz;

[Generator]
public sealed class Hanz : IIncrementalGenerator
{
    internal static ILogger Logger = NullLogger.Instance;

    public record struct LoggingOptions(string FilePath, LogLevel Level);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var entityHierarchies = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => node is ClassDeclarationSyntax {AttributeLists.Count: > 0},
            transform: static (ctx, _) => EntityHierarchies.GetTargetForGeneration(ctx)
        );

        var proxyInterfaces = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => InterfaceProxy.IsTarget(node),
            transform: static (ctx, _) => InterfaceProxy.GetTargetForGeneration(ctx)
        );

        var typeFactories = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => TypeFactories.IsTarget(node),
            transform: static (ctx, _) => TypeFactories.GetTargetForGeneration(ctx)
        );

        var varargs = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => VariableFuncArgs.IsValid(node),
            transform: static (ctx, _) => VariableFuncArgs.GetGenerationTarget(ctx)
        ).Collect();

        var restLoadable = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => RestLoadableSource.IsValid(node),
            transform: static (ctx, _) => RestLoadableSource.GetTargetForGeneration(ctx)
        );

        var sourceOfTruth = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => SourceOfTruth.IsValid(node),
            transform: static (ctx, _) => SourceOfTruth.GetTargetForGeneration(ctx)
        ).Collect();

        var intrinsicOverride = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => CovariantOverride.IsValid(node),
            transform: static (ctx,_) => CovariantOverride.GetTargetForGeneration(ctx)
        ).Collect();

        var modelEquality = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => ModelEquality.IsValid(node),
            transform: static (ctx, _) => ModelEquality.GetTargetForGeneration(ctx)
        );

        context.RegisterSourceOutput(
            entityHierarchies,
            EntityHierarchies.Execute
        );

        context.RegisterSourceOutput(
            proxyInterfaces,
            InterfaceProxy.Execute
        );

        context.RegisterSourceOutput(
            typeFactories,
            TypeFactories.Execute
        );

        context.RegisterSourceOutput(
            varargs,
            VariableFuncArgs.Execute
        );

        context.RegisterSourceOutput(
            restLoadable,
            RestLoadableSource.Execute
        );

        context.RegisterSourceOutput(
            sourceOfTruth,
            SourceOfTruth.Execute
        );

        context.RegisterSourceOutput(
            intrinsicOverride,
            CovariantOverride.Execute
        );

        context.RegisterSourceOutput(
            modelEquality,
            ModelEquality.Execute
        );

        var options = context.AnalyzerConfigOptionsProvider.Select((options, _) => GetLoggingOptions(options));

        SetupLogger(context, options);
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
