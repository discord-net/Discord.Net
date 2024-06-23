using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace Discord.Net.SourceGenerators;

public abstract class DiscordSourceGenerator : ISourceGenerator
{
    private readonly StringBuilder _initLog = new();
    protected readonly StringBuilder _executeLog = new();

    private StringBuilder? _currentLog;

    public virtual void OnInitialize(in GeneratorInitializationContext context)
    {}

    public abstract void OnExecute(in GeneratorExecutionContext context);

    public void Initialize(GeneratorInitializationContext context)
    {
        _initLog.Clear();
        _currentLog = _initLog;
        Log("Preforming initialization...");
        OnInitialize(in context);
    }

    public void Execute(GeneratorExecutionContext context)
    {
        _executeLog.Clear();
        _currentLog = _executeLog;
        Log("Starting execute...");
        try
        {
            OnExecute(in context);
        }
        catch (Exception x)
        {
            Log("Error occured: ");
            Log(x.ToString());
        }
        finally
        {
            context.AddSource("log.g.cs", $"{_initLog}\n{_executeLog}");
        }
    }

    protected void Log(string message)
    {
        _currentLog!.AppendLine($"// {message.Replace("\n", "\n // ")}");
    }
}
