using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz;

public abstract class GenerationTask
{
    private static readonly Dictionary<Type, GenerationTask> _tasks = [];

    private static readonly Logger _logger = Logger.CreateForTask("GenerationTaskBuilder").WithCleanLogFile();
    
    public GenerationTask(Context context, Logger logger)
    {
    }

    public static void Initialize(IncrementalGeneratorInitializationContext context)
    {
        _logger.Clean();
        
        var queue = new Queue<Type>(
            typeof(GenerationTask).Assembly.GetTypes()
                .Where(x => !x.IsAbstract && typeof(GenerationTask).IsAssignableFrom(x))
        );

        var taskContext = new Context(context);

        _logger.Log($"{queue.Count} tasks to initialize...");
        
        while (queue.Count > 0)
        {
            var type = queue.Dequeue();
            
            _logger.Log($"Initializing {type}...");

            if (_tasks.ContainsKey(type)) continue;

            var taskLogger = Logger.CreateForTask(type.Name).WithCleanLogFile();
            
            _tasks[type] = (GenerationTask) Activator.CreateInstance(
                type, taskContext, taskLogger
            );
            
            taskLogger.Flush();
        }
        
        _logger.Flush();
    }

    private static T GetOrCreate<T>(Context context) where T : GenerationTask
    {
        if (_tasks.TryGetValue(typeof(T), out var rawTask))
        {
            if (rawTask is not T task)
                throw new InvalidCastException();

            return task;
        }

        var logger = Hanz.DefaultLogger.GetSubLogger(typeof(T).Name);

        var instance = (T) Activator.CreateInstance(typeof(T), context, logger);
        _tasks[typeof(T)] = instance;
        return instance;
    }

    public readonly struct Context
    {
        public readonly IncrementalGeneratorInitializationContext GeneratorContext;

        public Context(IncrementalGeneratorInitializationContext context)
        {
            GeneratorContext = context;
        }

        public T GetTask<T>() where T : GenerationTask
            => GetOrCreate<T>(this);
    }
}