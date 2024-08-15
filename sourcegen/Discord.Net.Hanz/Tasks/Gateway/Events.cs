using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using System.Globalization;

namespace Discord.Net.Hanz.Tasks.Gateway;

[Flags]
public enum EventParameterDegree : byte
{
    None = 0,
    Id = 1 << 0,
    Model = 1 << 1,
    Actor = 1 << 2,
    Handle = 1 << 3,
    Entity = 1 << 4,

    All = Id | Model | Actor | Handle | Entity,

    Unknown = byte.MaxValue
}

public sealed class Events : IGenerationCombineTask<Events.GenerationTarget>
{
    public abstract class GenerationTarget : IEquatable<GenerationTarget>
    {
        public abstract bool Equals(GenerationTarget other);
    }

    public sealed class ProcessorGenerationTarget(
        SemanticModel model,
        ClassDeclarationSyntax syntax,
        INamedTypeSymbol symbol,
        INamedTypeSymbol payload,
        string dispatchName
    ) : GenerationTarget, IEquatable<ProcessorGenerationTarget>
    {
        public SemanticModel Model { get; } = model;
        public ClassDeclarationSyntax Syntax { get; } = syntax;
        public INamedTypeSymbol Symbol { get; } = symbol;
        public INamedTypeSymbol Payload { get; } = payload;
        public string DispatchName { get; } = dispatchName;

        public bool Equals(ProcessorGenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Symbol.Equals(other.Symbol, SymbolEqualityComparer.Default);
        }

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is ProcessorGenerationTarget other && Equals(other);

        public override int GetHashCode() => SymbolEqualityComparer.Default.GetHashCode(Symbol);

        public override bool Equals(GenerationTarget other) =>
            other is ProcessorGenerationTarget generator && Equals(generator);

        public static bool operator ==(ProcessorGenerationTarget? left, ProcessorGenerationTarget? right) =>
            Equals(left, right);

        public static bool operator !=(ProcessorGenerationTarget? left, ProcessorGenerationTarget? right) =>
            !Equals(left, right);
    }

    public sealed class EventGenerationTarget : GenerationTarget, IEquatable<EventGenerationTarget>
    {
        public SemanticModel SemanticModel { get; }
        public ClassDeclarationSyntax EventClassSyntax { get; }
        public INamedTypeSymbol EventClass { get; }
        public INamedTypeSymbol Package { get; }
        public INamedTypeSymbol Payload { get; }
        public INamedTypeSymbol PackagePayload { get; }
        public Dictionary<AttributeData, INamedTypeSymbol> SubscribableAttributes { get; }
        public string DispatchName { get; }

        public EventGenerationTarget(
            SemanticModel semanticModel,
            ClassDeclarationSyntax eventClassSyntax,
            INamedTypeSymbol eventClass,
            INamedTypeSymbol package,
            INamedTypeSymbol payload,
            Dictionary<AttributeData, INamedTypeSymbol> subscribableAttributes,
            string dispatchName)
        {
            SemanticModel = semanticModel;
            EventClassSyntax = eventClassSyntax;
            EventClass = eventClass;
            Package = package;
            Payload = payload;
            SubscribableAttributes = subscribableAttributes;
            DispatchName = dispatchName;

            PackagePayload = Package
                    .Interfaces
                    .FirstOrDefault(x => x.Name == "IDispatchPackage")
                is {IsGenericType: true} dispatchPackageInterface
                ? (INamedTypeSymbol)dispatchPackageInterface.TypeArguments[0]
                : Payload;
        }

        public bool Equals(EventGenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventClass.Equals(other.EventClass, SymbolEqualityComparer.Default) &&
                   DispatchName == other.DispatchName;
        }

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is EventGenerationTarget other && Equals(other);

        public override int GetHashCode() => SymbolEqualityComparer.Default.GetHashCode(EventClass);

        public override bool Equals(GenerationTarget other) =>
            other is EventGenerationTarget generator && Equals(generator);

        public static bool operator ==(EventGenerationTarget? left, EventGenerationTarget? right) =>
            Equals(left, right);

        public static bool operator !=(EventGenerationTarget? left, EventGenerationTarget? right) =>
            !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
        => node is ClassDeclarationSyntax cls && cls.AttributeLists.Any(x => x.Attributes.Count > 0);

    private static bool TryGetDispatchEventName(ITypeSymbol type, out string name)
    {
        var attribute = type.GetAttributes()
            .FirstOrDefault(x =>
                x.AttributeClass?.ToDisplayString() == "Discord.DispatchEventAttribute"
            );

        if (attribute is null)
        {
            name = null!;
            return false;
        }

        return (name = (attribute.ConstructorArguments[0].Value as string)!) is not null;
    }

    public GenerationTarget? GetTargetForGeneration(
        GeneratorSyntaxContext context,
        Logger logger,
        CancellationToken token = default)
    {
        if (context.Node is not ClassDeclarationSyntax syntax) return null;

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, syntax) is not INamedTypeSymbol symbol)
            return null;

        if (!TryGetDispatchEventName(symbol, out var dispatchName))
            return null;

        if (symbol.Interfaces.FirstOrDefault(x =>
                x.ToDisplayString().StartsWith("Discord.Gateway.Processors.IDispatchProcessor<"))
            is { } processor)
        {
            return new ProcessorGenerationTarget(
                context.SemanticModel,
                syntax,
                symbol,
                (processor.TypeArguments[0] as INamedTypeSymbol)!,
                dispatchName
            );
        }

        if (
            symbol.BaseType is not
            {
                Name: "DispatchEvent", IsGenericType: true
            } dispatchEvent
        ) return null;


        var subscribableEvents = symbol.GetAttributes()
            .Where(x =>
                x.AttributeClass?.ToDisplayString().StartsWith("Discord.SubscribableAttribute") ?? false
            )
            .ToDictionary(x => x, x => (INamedTypeSymbol)x.AttributeClass!.TypeArguments[0]);

        if (subscribableEvents.Count == 0) return null;

        if (
            dispatchEvent.TypeArguments[0] is not INamedTypeSymbol package ||
            dispatchEvent.TypeArguments[1] is not INamedTypeSymbol payload
        )
            return null;

        return new EventGenerationTarget(
            context.SemanticModel,
            syntax,
            symbol,
            package,
            payload,
            subscribableEvents,
            dispatchName
        );
    }

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets, Logger logger)
    {
        if (targets.Length == 0) return;

        ExecuteEvents(context, targets.OfType<EventGenerationTarget>(), logger);
        ExecuteProcessors(context, targets.OfType<ProcessorGenerationTarget>(), logger);
    }

    private static void ExecuteProcessors(
        SourceProductionContext context,
        IEnumerable<ProcessorGenerationTarget> targets,
        Logger logger)
    {
        var processorGenerationTargets = targets as ProcessorGenerationTarget[] ?? targets.ToArray();

        if (processorGenerationTargets.Length == 0) return;

        var clientClassSyntax = SyntaxFactory.ClassDeclaration(
            [],
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.SealedKeyword),
                SyntaxFactory.Token(SyntaxKind.PartialKeyword)
            ),
            SyntaxFactory.Identifier("DiscordGatewayClient"),
            null,
            null,
            [],
            []
        );

        var processedSymbols = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var target in processorGenerationTargets)
        {
            if (!processedSymbols.Add(target.Symbol))
                continue;

            var companionSyntax = SyntaxUtils.CreateSourceGenClone(target.Syntax)
                .AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"public string DispatchEventType => \"{target.DispatchName}\";"
                    )!
                );

            context.AddSource(
                $"EventProcessors/{target.Symbol.Name}",
                $$"""
                  {{target.Syntax.GetFormattedUsingDirectives()}}

                  namespace {{target.Symbol.ContainingNamespace}};

                  {{companionSyntax.NormalizeWhitespace()}}
                  """
            );
        }

        clientClassSyntax = clientClassSyntax.AddMembers(
            SyntaxFactory.ParseMemberDeclaration(
                """
                internal Dictionary<string, HashSet<IDispatchProcessor>> DispatchProcessors
                    => _dispatchProcessors ??= CreateProcessorsMap();
                """
            )!,
            SyntaxFactory.ParseMemberDeclaration(
                "private Dictionary<string, HashSet<IDispatchProcessor>>? _dispatchProcessors;"
            )!,
            SyntaxFactory.ParseMemberDeclaration(
                """
                internal HashSet<IDispatchProcessor> GetProcessors(string eventName)
                    => DispatchProcessors.TryGetValue(eventName, out var set) ? set : [];
                """
            )!,
            SyntaxFactory.ParseMemberDeclaration(
                $$"""
                  private Dictionary<string, HashSet<IDispatchProcessor>> CreateProcessorsMap()
                  {
                      var map = new Dictionary<string, HashSet<IDispatchProcessor>>()
                      {
                          {{
                              string.Join(
                                  ",\n",
                                  processorGenerationTargets.Select(x =>
                                      $$"""
                                        { "{{x.DispatchName}}", [new {{x.Symbol}}(this)] }
                                        """
                                  )
                              )
                          }}
                      };

                      foreach(var userProcessor in Config.EventProcessors)
                      {
                          var instance = userProcessor.Get(this);

                          if(!map.TryGetValue(instance.DispatchEventType, out var set))
                              map[instance.DispatchEventType] = set = new();

                          set.Add(instance);
                      }
                      
                      return map;
                  }
                  """
            )!
        );

        context.AddSource(
            "EventProcessors/Client",
            $$"""
              using Discord.Gateway.Processors;
              using Discord.Models;

              namespace Discord.Gateway;

              {{clientClassSyntax.NormalizeWhitespace()}}
              """
        );
    }

    private static void ExecuteEvents(
        SourceProductionContext context,
        IEnumerable<EventGenerationTarget> targets,
        Logger logger)
    {
        var eventGeneratorTargets = targets as EventGenerationTarget[] ?? targets.ToArray();

        if (eventGeneratorTargets.Length == 0) return;

        var generatedEvents = new List<EventGenerationTarget>();

        var addedEvents = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var target in eventGeneratorTargets)
        {
            if (target is null) continue;

            var targetLogger = logger.WithSemanticContext(target.SemanticModel);

            if (target.Package.DeclaringSyntaxReferences.Length == 0) continue;

            if (
                target.Package.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax()
                is not ClassDeclarationSyntax packageSyntax
            ) continue;

            packageSyntax = SyntaxUtils.CreateSourceGenClone(packageSyntax)
                .AddMembers(
                    SyntaxFactory.ParseMemberDeclaration($"public {target.EventClass} Handler {{ get; }}")!,
                    SyntaxFactory.ParseMemberDeclaration($"public {target.PackagePayload} Payload {{ get; }}")!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          public {{target.Package.Name}}({{target.EventClass}} handler, {{target.PackagePayload}} payload)
                          {
                              Handler = handler;
                              Payload = payload;
                          }
                          """
                    )!
                );

            var handlerInterface = SyntaxFactory.InterfaceDeclaration(
                [],
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.InternalKeyword)
                ),
                SyntaxFactory.Identifier($"I{target.EventClass.Name}InternalPackagerDoNoUseOrYouWillBeFired"),
                null,
                null,
                [],
                []
            );

            var companionSyntax = SyntaxUtils.CreateSourceGenClone(target.EventClassSyntax)
                .AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(handlerInterface.Identifier))
                )
                .AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"public static string DispatchEventName => \"{target.DispatchName}\";"
                    )!
                );

            foreach (var eventDelegate in target.SubscribableAttributes.Values
                         .Where(x => x.DelegateInvokeMethod is not null))
            {
                targetLogger.Log($"{target.EventClass}: Adding {eventDelegate} delegate");

                companionSyntax = companionSyntax.AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName($"ISubscribableDispatchEvent<{eventDelegate}>")
                    )
                );

                AddDelegateToPackage(
                    ref packageSyntax,
                    ref companionSyntax,
                    ref handlerInterface,
                    eventDelegate.DelegateInvokeMethod!,
                    target,
                    targetLogger
                );
            }

            if (!addedEvents.Add(target.EventClass))
                continue;

            context.AddSource(
                $"Events/{target.EventClass.Name}",
                $$"""
                  {{target.EventClassSyntax.GetFormattedUsingDirectives()}}

                  namespace {{target.EventClass.ContainingNamespace}};

                  {{packageSyntax.NormalizeWhitespace()}}

                  {{handlerInterface.NormalizeWhitespace()}}

                  {{companionSyntax.NormalizeWhitespace()}}
                  """
            );

            generatedEvents.Add(target);
        }

        if (generatedEvents.Count == 0) return;

        var clientClassSyntax = SyntaxFactory.ClassDeclaration(
            [],
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.SealedKeyword),
                SyntaxFactory.Token(SyntaxKind.PartialKeyword)
            ),
            SyntaxFactory.Identifier("DiscordGatewayClient"),
            null,
            null,
            [],
            []
        );

        var eventMap = new Dictionary<string, HashSet<string>>();

        foreach (var generated in generatedEvents)
        {
            var propName = generated.EventClass.Name;

            if (!propName.EndsWith("Event"))
                propName += "Event";

            var eventName = generated.EventClass.Name;

            if (eventName.EndsWith("Event"))
                eventName = eventName.Remove(eventName.Length - 5, 5);

            clientClassSyntax = clientClassSyntax.AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    $"public {generated.EventClass.ToDisplayString()} {propName} {{ get; private set; }}"
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      public event {{generated.SubscribableAttributes.Values.First()}} {{eventName}}
                      {
                          add => {{propName}}.Subscribe(value);
                          remove => {{propName}}.Unsubscribe(value);
                      }
                      """
                )!
            );

            if (!eventMap.TryGetValue(generated.DispatchName, out var set))
                eventMap[generated.DispatchName] = set = new HashSet<string>();

            set.Add(propName);
        }

        clientClassSyntax = clientClassSyntax.AddMembers(
            SyntaxFactory.ParseMemberDeclaration(
                $$"""
                  public HashSet<IDispatchEvent>? GetDispatchEvents(string eventName)
                  {
                      return eventName switch
                      {
                          {{
                              string.Join(
                                  "\n",
                                  eventMap.Select(x =>
                                      $"\"{x.Key}\" => [{string.Join(", ", x.Value.Select(x => $"this.{x}"))}],"
                                  )
                              )
                          }}
                          _ => null
                      };
                  }
                  """
            )!,
            SyntaxFactory.ParseMemberDeclaration(
                $$"""
                  [{{
                      string.Join(
                          ",",
                          eventMap.Values
                              .SelectMany(x => x
                                  .Select(x => $"MemberNotNull(nameof({x}))")
                              )
                      )
                  }}]
                  private void InitializeEvents()
                  {
                      {{
                          string.Join(
                              "\n",
                              eventMap.Values.SelectMany(x => x
                                  .Select(x => $"this.{x} ??= new(this);")
                              )
                          )
                      }}
                  }
                  """
            )!
        );

        context.AddSource(
            "Events/Client",
            $"""
             using Discord;
             using System.Diagnostics.CodeAnalysis;

             namespace Discord.Gateway;

             {clientClassSyntax.NormalizeWhitespace()}
             """
        );
    }

    private readonly struct ParsedParameter(ITypeSymbol parameterType, string parameterName, string value)
        : IEquatable<ParsedParameter>
    {
        public readonly ITypeSymbol ParameterType = parameterType;
        public readonly string ParameterName = parameterName;
        public readonly string Value = value;

        public bool Equals(ParsedParameter other)
            => ParameterType.Equals(other.ParameterType, SymbolEqualityComparer.Default) &&
               ParameterName == other.ParameterName && Value == other.Value;

        public override bool Equals(object? obj) => obj is ParsedParameter other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SymbolEqualityComparer.Default.GetHashCode(ParameterType);
                hashCode = (hashCode * 397) ^ ParameterName.GetHashCode();
                hashCode = (hashCode * 397) ^ Value.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ParsedParameter left, ParsedParameter right) => left.Equals(right);

        public static bool operator !=(ParsedParameter left, ParsedParameter right) => !left.Equals(right);
    }

    private static void AddDelegateToPackage(
        ref ClassDeclarationSyntax packageSyntax,
        ref ClassDeclarationSyntax eventClassSyntax,
        ref InterfaceDeclarationSyntax handlerInterface,
        IMethodSymbol delegateMethod,
        EventGenerationTarget target,
        Logger logger)
    {
        var parsedParameters = delegateMethod.Parameters
            .ToDictionary<IParameterSymbol, Parameter, IParameterSymbol>(
                x => x,
                x => new Parameter(x, target.SemanticModel),
                SymbolEqualityComparer.Default
            );

        List<(IParameterSymbol Parameter, Dictionary<EventParameterDegree, string> Generated, EventParameterDegree
            ResolveDegree)> parameterMapping = new();

        foreach (var parameter in delegateMethod.Parameters)
        {
            var name = ToTitle(parameter.Name);

            var degree = GetImplicitDegree(parameter);

            logger.Log($" - {parameter.Type}: {name} -> {degree}");

            var mapping = new Dictionary<EventParameterDegree, string>();
            var requiredResolveDegree = EventParameterDegree.None;

            foreach (var implementedDegree in GetImplementedDegrees(parameter, degree))
            {
                if (requiredResolveDegree is EventParameterDegree.Unknown)
                    break;

                logger.Log($" - {parameter}: Resolved degree {implementedDegree}");

                switch (implementedDegree)
                {
                    case EventParameterDegree.None:
                        mapping.Add(EventParameterDegree.None, $"await Get{name}Async(token)");
                        requiredResolveDegree = EventParameterDegree.Unknown;
                        break;
                    case EventParameterDegree.Id when
                        TryGetModelType(parameter.Type, out var modelType) &&
                        target.SemanticModel.Compilation.HasImplicitConversion(
                            target.PackagePayload,
                            modelType
                        ):
                        mapping.Add(EventParameterDegree.Id, "package.Payload.Id");
                        break;
                    case EventParameterDegree.Id:
                        mapping.Add(EventParameterDegree.Id, $"package.Get{name}Id()");
                        requiredResolveDegree |= EventParameterDegree.Id;
                        break;
                    case EventParameterDegree.Model when
                        TryGetModelType(parameter.Type, out var modelType) &&
                        target.SemanticModel.Compilation.HasImplicitConversion(
                            target.PackagePayload,
                            modelType
                        ):
                        mapping.Add(EventParameterDegree.Model, "package.Payload");
                        break;
                    case EventParameterDegree.Model:
                        mapping.Add(EventParameterDegree.Model, $"package.Get{name}Model()");
                        requiredResolveDegree |= EventParameterDegree.Model;
                        break;
                    case EventParameterDegree.Actor:
                        mapping.Add(EventParameterDegree.Actor, $"package.Get{name}Actor()");
                        requiredResolveDegree |= EventParameterDegree.Actor;
                        break;
                    case EventParameterDegree.Handle:
                        mapping.Add(EventParameterDegree.Handle, $"await package.Get{name}HandleAsync(token)");
                        requiredResolveDegree |= EventParameterDegree.Handle;
                        break;
                    case EventParameterDegree.Entity:
                        mapping.Add(EventParameterDegree.Entity,
                            $"(await package.Get{name}HandleAsync(token)).ConsumeAsReference()");
                        requiredResolveDegree |= EventParameterDegree.Handle;
                        break;
                }
            }

            parameterMapping.Add((parameter, mapping, requiredResolveDegree));
        }

        foreach (var mapping in parameterMapping)
        {
            if (mapping.ResolveDegree is EventParameterDegree.None) continue;

            foreach (var requiredDegree in Enum.GetValues(typeof(EventParameterDegree))
                         .Cast<EventParameterDegree>()
                         .Where(x => mapping.ResolveDegree.HasFlag(x)))
            {
                if (requiredDegree is EventParameterDegree.None) continue;

                if (!parsedParameters[mapping.Parameter].TryGetType(requiredDegree, out var type))
                {
                    logger.Warn($" - Skipping {mapping.Parameter.Name} {requiredDegree}: No type info");
                    continue;
                }

                logger.Log(
                    $" - Adding get method for {mapping.Parameter.Name}: {requiredDegree} ({mapping.ResolveDegree})"
                );

                AddEventParameter(
                    ref handlerInterface,
                    ref packageSyntax,
                    parsedParameters[mapping.Parameter],
                    type,
                    requiredDegree,
                    target.PackagePayload,
                    logger
                );
            }
        }

        List<ParsedParameter?[]> processingMapping = new();

        for (var i = 0; i < parameterMapping.Count; i++)
        {
            var mapping = parameterMapping[i];
            var parsedParameter = new Parameter(mapping.Parameter, target.SemanticModel);


            foreach (var parameter in mapping.Generated)
            {
                if (!parsedParameter.TryGetType(parameter.Key, out var type))
                    continue;

                logger.Log($"  -> {parameter.Key} : {parsedParameter}");


                var entry = new ParsedParameter(type, parsedParameter.ParameterSymbol.Name, parameter.Value);

                var toUpdate = processingMapping
                    .Where(x => x[i] is null)
                    .ToArray();

                if (toUpdate.Length == 0)
                {
                    var arr = new ParsedParameter?[parameterMapping.Count];
                    arr[i] = entry;
                    processingMapping.Add(arr);
                    continue;
                }

                foreach (var targetToUpdate in toUpdate)
                {
                    var arr = new ParsedParameter?[parameterMapping.Count];
                    targetToUpdate.CopyTo(arr, 0);
                    arr[i] = entry;
                    processingMapping.Add(arr);
                }
            }
        }

        logger.Log($"Processed mapping: {processingMapping.Count} variants");

        var finalizedMapping = processingMapping
            .Where(x => x.All(x => x.HasValue))
            .Select(x => x.Select(x => x!.Value).ToArray())
            .ToImmutableHashSet();

        logger.Log($"Finalized mapping: {finalizedMapping.Count} variants");

        foreach (var entry in finalizedMapping)
        {
            logger.Log($" -> {string.Join(", ", entry
                .Select(x => $"{x.ParameterType} {x.ParameterName} = {x.Value}"))}");
        }

        var delegateMapping = finalizedMapping
            .FirstOrDefault(x => x
                .Select((x, i) => (x, i))
                .All(x => delegateMethod.Parameters[x.i].Type
                    .Equals(x.x.ParameterType, SymbolEqualityComparer.Default)
                )
            );

        if (delegateMapping is null)
        {
            logger.Warn($"Cannot find default mapping for {delegateMethod}");
            return;
        }

        var delegateHasCancelToken = delegateMethod.Parameters.Last().Type.Name == "CancellationToken";

        AddSubscribable(
            ref eventClassSyntax,
            target.Package,
            delegateMethod.ContainingType.ToDisplayString(),
            delegateMapping,
            delegateHasCancelToken,
            true
        );

        foreach (var mapping in finalizedMapping)
        {
            var genericsParameters = string.Join(", ", mapping.Select(x => x.ParameterType.ToDisplayString()));

            var sigs = new List<(string Delegate, bool IsAsync, bool HasCancellationToken)>();

            if (delegateMapping == mapping)
            {
                var counterReturnType = delegateMethod.ReturnType is {Name: "ValueTask"}
                    ? "Task"
                    : "ValueTask";

                sigs.Add(($"Func<{genericsParameters}, {counterReturnType}>", true, false));

                if (!delegateHasCancelToken)
                {
                    sigs.Add(($"Func<{genericsParameters}, CancellationToken, ValueTask>", true, true));
                    sigs.Add(($"Func<{genericsParameters}, CancellationToken, Task>", true, true));
                }

                sigs.Add(($"Action<{genericsParameters}>", false, false));
            }
            else
            {
                sigs.AddRange([
                    ($"Func<{genericsParameters}, CancellationToken, ValueTask>", true, true),
                    ($"Func<{genericsParameters}, CancellationToken, Task>",true,  true),
                    ($"Func<{genericsParameters}, ValueTask>", true, false),
                    ($"Func<{genericsParameters}, Task>", true, false),
                    ($"Action<{genericsParameters}>", false, false)
                ]);
            }

            foreach (var sig in sigs)
            {
                AddSubscribable(
                    ref eventClassSyntax,
                    target.Package,
                    sig.Delegate,
                    mapping,
                    sig.HasCancellationToken,
                    sig.IsAsync
                );
            }
        }
    }

    static void AddSubscribable(
        ref ClassDeclarationSyntax target,
        ITypeSymbol package,
        string delegateSig,
        ParsedParameter[] mapping,
        bool hasCancelToken,
        bool isAsync)
    {
        var shouldBeAsync = mapping.Any(x => x.Value.Contains("await"));
        
        if (!isAsync && shouldBeAsync)
            return;
        
        var parameters = string.Join(", ", mapping.Select(x => x.Value));

        if (hasCancelToken)
            parameters = mapping.Length == 0 ? "token" : $"{parameters}, token";

        var invokeBody = $"handler({parameters})";

        if (isAsync)
            invokeBody = $"await {invokeBody}";

        if (delegateSig.StartsWith("Action"))
            invokeBody = $"{{ {invokeBody}; return ValueTask.CompletedTask; }}";

        target = target
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.IdentifierName($"ISubscribableEvent<{delegateSig}>")
                )
            )
            .AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      public void Subscribe({{delegateSig}} handler)
                          => AddSubscriber(handler, {{(isAsync ? "async " : string.Empty)}}({{package}} package, CancellationToken token) => {{invokeBody}});
                      """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      public void Unsubscribe({{delegateSig}} handler)
                          => RemoveSubscriber(handler);
                      """
                )!
            );
    }

    private readonly record struct Parameter
    {
        public readonly EventParameterDegree ImplementedDegree;
        public readonly EventParameterDegree SupportedDegree;

        public readonly IParameterSymbol ParameterSymbol;

        public readonly ITypeSymbol? IdType;
        public readonly ITypeSymbol? ModelType;
        public readonly ITypeSymbol? ActorType;
        public readonly ITypeSymbol? EntityType;
        public readonly ITypeSymbol? HandleType;

        public bool TryGetType(EventParameterDegree degree, out ITypeSymbol type)
        {
            switch (degree)
            {
                case EventParameterDegree.Id:
                    return (type = IdType!) is not null;
                case EventParameterDegree.Model:
                    return (type = ModelType!) is not null;
                case EventParameterDegree.Actor:
                    return (type = ActorType!) is not null;
                case EventParameterDegree.Handle:
                    return (type = HandleType!) is not null;
                case EventParameterDegree.Entity:
                    return (type = EntityType!) is not null;
            }

            type = null!;
            return false;
        }

        public Parameter(IParameterSymbol parameter, SemanticModel semanticModel)
        {
            ImplementedDegree = GetImplicitDegree(parameter);
            SupportedDegree = GetSupportedDegree(parameter);
            ParameterSymbol = parameter;

            switch (ImplementedDegree)
            {
                case EventParameterDegree.Id:
                    IdType = parameter.Type;
                    break;
                case EventParameterDegree.Model when TryGetIdType(parameter.Type, out IdType):
                    ModelType = parameter.Type;
                    break;
                case EventParameterDegree.Actor when
                    TryGetModelType(parameter.Type, out ModelType) &&
                    TryGetIdType(ModelType, out IdType) &&
                    TryGetGatewayEntityType(parameter.Type, semanticModel, out EntityType):
                    ActorType = parameter.Type;
                    break;
                case EventParameterDegree.Handle when
                    parameter.Type is INamedTypeSymbol namedTypeSymbol &&
                    TryGetModelType(namedTypeSymbol.TypeArguments[1], out ModelType) &&
                    TryGetIdType(ModelType, out IdType) &&
                    TryGetGatewayActorType(namedTypeSymbol.TypeArguments[1], semanticModel, out ActorType):
                    HandleType = parameter.Type;
                    EntityType = namedTypeSymbol.TypeArguments[1];
                    break;
                case EventParameterDegree.Entity when
                    TryGetModelType(parameter.Type, out ModelType) &&
                    TryGetIdType(ModelType, out IdType) &&
                    TryGetGatewayActorType(parameter.Type, semanticModel, out ActorType) &&
                    semanticModel.Compilation.GetTypeByMetadataName("Discord.Gateway.IEntityHandle`2") is { } handle:
                    EntityType = parameter.Type;
                    HandleType = handle.Construct(IdType, parameter.Type);
                    break;
            }
        }
    }

    private static IEnumerable<EventParameterDegree> GetImplementedDegrees(
        IParameterSymbol parameter,
        EventParameterDegree? parameterDegree = null)
    {
        parameterDegree ??= GetImplicitDegree(parameter);
        var supported = GetSupportedDegree(parameter);

        var rawDegree = (byte)parameterDegree.Value;

        while (rawDegree > 0)
        {
            yield return (EventParameterDegree)rawDegree;

            while (rawDegree > 0)
            {
                rawDegree >>= 1;
                if (supported.HasFlag((EventParameterDegree)rawDegree))
                    break;
            }
        }
    }

    private static void AddEventParameter(
        ref InterfaceDeclarationSyntax handlerInterface,
        ref ClassDeclarationSyntax packageSyntax,
        Parameter parameter,
        ITypeSymbol type,
        EventParameterDegree degree,
        INamedTypeSymbol payload,
        Logger logger)
    {
        var name = ToTitle(parameter.ParameterSymbol.Name);

        var isAsync = degree is EventParameterDegree.Unknown or EventParameterDegree.Handle;
        var getName = $"Get{name}" + degree switch
        {
            EventParameterDegree.Id => "Id",
            EventParameterDegree.Model => "Model",
            EventParameterDegree.Actor => "Actor",
            EventParameterDegree.Handle => "Handle",
            _ => string.Empty
        };

        if (isAsync) getName += "Async";

        logger.Log($" - {parameter.ParameterSymbol.Type}: {name} -> {getName} ({degree})");

        var returnType = type.ToDisplayString();

        var returnResult = isAsync
            ? $"ValueTask<{returnType}>"
            : returnType;

        var cancelTokenAdditional = isAsync
            ? ", CancellationToken token"
            : string.Empty;

        var cancelToken = isAsync
            ? "CancellationToken token"
            : string.Empty;

        var cancelTokenArg = isAsync
            ? ", token"
            : string.Empty;

        var shouldCache = degree is not EventParameterDegree.Handle;
        var cacheFieldName = shouldCache ? $"_{parameter.ParameterSymbol.Name}{degree}" : string.Empty;

        var cacheAccess = shouldCache ? $"{cacheFieldName} ??= " : string.Empty;

        handlerInterface = handlerInterface.AddMembers(
            SyntaxFactory.ParseMemberDeclaration(
                $"{returnResult} {getName}({payload} payload{cancelTokenAdditional});"
            )!
        );

        if (shouldCache)
        {
            packageSyntax = packageSyntax.AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    $"private {returnType}{(returnType.EndsWith("?") ? string.Empty : "?")} {cacheFieldName};"
                )!
            );
        }

        packageSyntax = packageSyntax.AddMembers(
            SyntaxFactory.ParseMemberDeclaration(
                $"public{(isAsync ? " async" : string.Empty)} {returnResult} {getName}({cancelToken}) => {cacheAccess}{(isAsync ? "await " : string.Empty)}Handler.{getName}(Payload{cancelTokenArg});"
            )!
        );
    }

    private static EventParameterDegree GetImplicitDegree(IParameterSymbol parameter)
    {
        if (parameter.Type is {Name: "ulong" or "string"})
            return EventParameterDegree.Id;

        if (IsModelType(parameter.Type))
            return EventParameterDegree.Model;

        if (IsGatewayActor(parameter.Type, out _))
            return EventParameterDegree.Actor;

        if (parameter.Type.ToDisplayString().StartsWith("Discord.Gateway.IEntityHandle<"))
            return EventParameterDegree.Handle;

        if (IsGatewayEntity(parameter.Type, out _))
            return EventParameterDegree.Entity;

        return EventParameterDegree.None;
    }

    private static bool IsModelType(ITypeSymbol symbol)
        => symbol.AllInterfaces.Any(x => x.ToDisplayString() == "Discord.Models.IModel");

    private static EventParameterDegree GetSupportedDegree(IParameterSymbol parameter)
    {
        var attribute = parameter.GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == "Discord.SupportsAttribute");

        if (attribute is null)
            return EventParameterDegree.All;

        return (EventParameterDegree)attribute.ConstructorArguments[0].Value!;
    }

    private static string ToTitle(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
    }

    private static bool TryGetIdType(ITypeSymbol type, out ITypeSymbol idType)
    {
        idType = Hierarchy.GetHierarchy(type).FirstOrDefault(x =>
            x.Type.ToDisplayString().StartsWith("Discord.Models.IEntityModel<") ||
            x.Type.ToDisplayString().StartsWith("Discord.IActor<")
        ).Type?.TypeArguments[0]!;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        return idType is not null;
    }

    private static bool TryGetModelType(ITypeSymbol type, out ITypeSymbol modelType)
    {
        var hierarchy = Hierarchy.GetHierarchy(type);

        var entityOfModel = hierarchy
            .FirstOrDefault(x => x.Type.ToDisplayString().StartsWith("Discord.IEntityOf<"))
            .Type?.TypeArguments[0];

        if (entityOfModel is not null)
        {
            modelType = entityOfModel;
            return true;
        }

        var actorToEntity = hierarchy
            .FirstOrDefault(x => x.Type.ToDisplayString().StartsWith("Discord.IActor<"))
            .Type?.TypeArguments[1];

        if (actorToEntity is not null)
        {
            entityOfModel = Hierarchy.GetHierarchy(actorToEntity)
                .FirstOrDefault(x => x.Type.ToDisplayString().StartsWith("Discord.IEntityOf<"))
                .Type?.TypeArguments[0];

            if (entityOfModel is not null)
            {
                modelType = entityOfModel;
                return true;
            }
        }

        modelType = null!;
        return false;
    }

    private static bool IsGatewayActor(ITypeSymbol type, out ITypeSymbol actorType)
    {
        actorType = null!;

        var gatewayActorType = TypeUtils
                .GetBaseTypes(type)
                .FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.Gateway.GatewayActor<"))
            as INamedTypeSymbol;

        actorType = gatewayActorType!;

        return gatewayActorType is not null;
    }

    private static bool TryGetGatewayEntityType(ITypeSymbol actor, SemanticModel model, out ITypeSymbol entityType)
    {
        entityType = null!;

        if (!actor.Name.EndsWith("Actor")) return false;

        var name = actor.ToDisplayString();

        entityType = model.Compilation.GetTypeByMetadataName($"{name.Remove(name.Length - 5, 5)}")!;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        return entityType is not null;
    }

    private static bool TryGetGatewayActorType(ITypeSymbol type, SemanticModel semanticModel, out ITypeSymbol actor)
    {
        actor = null!;
        if (!IsGatewayEntity(type, out _)) return false;

        actor = semanticModel.Compilation.GetTypeByMetadataName($"{type.ToDisplayString()}Actor")!;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        return actor is not null;
    }

    private static bool IsGatewayEntity(ITypeSymbol type, out ITypeSymbol idType)
    {
        idType = null!;

        var gatewayEntityType = TypeUtils
                .GetBaseTypes(type)
                .FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.Gateway.GatewayEntity<"))
            as INamedTypeSymbol;

        if (gatewayEntityType is null) return false;

        idType = gatewayEntityType.TypeArguments[0];

        return true;
    }
}
