using System.Diagnostics;
using System.Reflection;
using Discord.Net.Hanz.Tasks.Actors.Links.V5;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz;

public abstract class GenerationTask
{
    private static readonly Dictionary<
        Type,
        Func<IncrementalGeneratorInitializationContext, Logger, GenerationTask>
    > _generators = new()
    {
        {typeof(LinksV5), (ctx, logger) => new LinksV5(ctx, logger)},
        {typeof(LinkActorTargets), (ctx, logger) => new LinkActorTargets(ctx, logger)},
        {typeof(LinkSchematics), (ctx, logger) => new LinkSchematics(ctx, logger)},
    };

    private static readonly Dictionary<Type, GenerationTask> _tasks = [];

    private static readonly Logger _logger = Logger.CreateForTask("GenerationTaskBuilder").WithCleanLogFile();

    public GenerationTask(IncrementalGeneratorInitializationContext context, Logger logger)
    {
    }

    public static void Initialize(IncrementalGeneratorInitializationContext context)
    {
        _tasks.Clear();
        _logger.Clean();

        try
        {
            var queue = new Queue<Type>(
                typeof(GenerationTask).Assembly.GetTypes()
                    .Where(x => !x.IsAbstract && typeof(GenerationTask).IsAssignableFrom(x))
            );

            _logger.Log($"{queue.Count} tasks to initialize...");

            while (queue.Count > 0)
            {
                var type = queue.Dequeue();

                _logger.Log($"Initializing {type}...");

                if (_tasks.ContainsKey(type) || !_generators.ContainsKey(type)) continue;

                var taskLogger = Logger.CreateForTask(type.Name).WithCleanLogFile();

                _tasks[type] = _generators[type](context, taskLogger);

                taskLogger.Flush();
            }
        }
        catch (Exception x)
        {
            _logger.Log($"Failed: {x}");
        }
        finally
        {
            _logger.Flush();
        }
    }

    public T GetTask<T>(IncrementalGeneratorInitializationContext context) where T : GenerationTask
        => GetOrCreate<T>(context);

    private static T GetOrCreate<T>(IncrementalGeneratorInitializationContext context) where T : GenerationTask
    {
        if (_tasks.TryGetValue(typeof(T), out var rawTask))
        {
            if (rawTask is not T task)
                throw new InvalidCastException();

            return task;
        }

        var logger = Logger.CreateForTask(typeof(T).Name).WithCleanLogFile();
        
        var instance = (T) _generators[typeof(T)](context, logger);
        _tasks[typeof(T)] = instance;
        
        logger.Flush();
        
        return instance;
    }
}