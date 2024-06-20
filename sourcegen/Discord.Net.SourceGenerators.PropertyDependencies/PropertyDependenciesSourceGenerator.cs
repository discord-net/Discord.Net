using Discord.Net.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discord.Net.SourceGenerators.PropertyDependencies;

[Generator]
public class PropertyDependenciesSourceGenerator : ISourceGenerator
{
    public const string TargetAttribute = "PropertyTransient";

    public void Initialize(GeneratorInitializationContext context) {}

    public void Execute(GeneratorExecutionContext context)
    {
        var log = new StringBuilder();

        try
        {
            var targets = Searching.FindPropertiesWithAttribute(ref context, TargetAttribute);

            foreach (var target in targets)
            {
                log.AppendLine($"// Processing {target.Identifier}");

                var semanticTarget = context.Compilation.GetSemanticModel(target.SyntaxTree);

                var properties = target
                    .DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>()
                    .Where(x => x.AttributeLists.SelectMany(x => x.Attributes)
                        .Any(x => x.Name.GetText().ToString() == TargetAttribute)
                    )
                    .ToList();

                log.AppendLine($"// - {properties.Count} properties found");

                var notifiersMap = new Dictionary<string, List<(string Identifier, string Value)>>();

                foreach (var property in properties)
                {
                    log.AppendLine($"// -- {property.Identifier}");

                    var attributes = property
                        .AttributeLists
                        .SelectMany(x => x.Attributes)
                        .Where(x => x.Name.GetText().ToString() == TargetAttribute)
                        .ToList();

                    log.AppendLine($"// -- Attributes {attributes.Count}");

                    ArgumentSyntax? PullArg(AttributeArgumentSyntax arg)
                    {
                        log.AppendLine($"// ---- {arg.GetText()}");

                        var argument = arg.DescendantNodes().OfType<ArgumentSyntax>().ToList();

                        if (argument.Count != 1)
                        {
                            log.AppendLine($"// ----- skipping {arg}, {argument.Count} argument syntax");
                            return null;
                        }

                        return argument[0];
                    }

                    foreach (var attribute in attributes)
                    {
                        log.AppendLine($"// --- {attribute.Name}");

                        var arguments = attribute
                            .DescendantNodes()
                            .OfType<AttributeArgumentSyntax>()
                            .ToList();

                        if (arguments.Count != 3)
                        {
                            log.AppendLine($"// --- skipping, has {arguments.Count} args");
                            continue;
                        }

                        var prop = PullArg(arguments[0])?.GetText().ToString();
                        var ident = PullArg(arguments[1])?.GetText().ToString();
                        var val = PullArg(arguments[2])?.GetText().ToString();

                        if (prop is null || ident is null || val is null)
                        {
                            log.AppendLine("// --- skipping, couldn't pull args");
                            continue;
                        }

                        if (!notifiersMap.TryGetValue(prop, out var map))
                            notifiersMap[prop] = map = new();

                        map.Add((ident, val));
                    }
                }

                foreach (var notifier in notifiersMap)
                {
                    log.AppendLine(
                        $"// [{notifier.Key}] {string.Join(", ", notifier.Value.Select(x => $"{x.Identifier} = {x.Value}"))}");



                    context.AddSource(
                        $"{target.Identifier}_NotifyPropertyTransient",
                        $$"""
                        #nullable enable
                        namespace {{semanticTarget.GetDeclaredSymbol(target)!.ContainingNamespace}};

                        public partial class {{target.Identifier}}
                        {
                            private void On{{notifier.Key}}Changed()
                            {
                                {{string.Join("\n        ", notifier.Value.Select(x => $"{x.Identifier} = {x.Value};"))}}
                            }
                        }
                        """
                        );
                }
            }
        }
        finally
        {
            context.AddSource("log.g.cs", log.ToString());
        }
    }
}
