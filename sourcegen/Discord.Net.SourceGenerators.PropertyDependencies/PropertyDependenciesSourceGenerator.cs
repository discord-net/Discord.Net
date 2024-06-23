using Discord.Net.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discord.Net.SourceGenerators.PropertyDependencies;

[Generator]
public class PropertyDependenciesSourceGenerator : DiscordSourceGenerator
{
    public const string TargetAttribute = "AssignOnPropertyChanged";

    public override void OnExecute(in GeneratorExecutionContext context)
    {
        var targets = Searching.FindPropertiesWithAttribute(in context, TargetAttribute);

        foreach (var target in targets)
        {
            Log($"Processing {target.Identifier}");

            var semanticTarget = context.Compilation.GetSemanticModel(target.SyntaxTree);

            var properties = target
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Where(x => x.AttributeLists.SelectMany(x => x.Attributes)
                    .Any(x => x.Name.GetText().ToString() == TargetAttribute)
                )
                .ToList();

            Log($"- {properties.Count} properties found");

            var notifiersMap = new Dictionary<string, List<(string Identifier, string Value)>>();

            foreach (var property in properties)
            {
                Log($"-- {property.Identifier}");

                var attributes = property
                    .AttributeLists
                    .SelectMany(x => x.Attributes)
                    .Where(x => x.Name.GetText().ToString() == TargetAttribute)
                    .ToList();

                Log($"-- Attributes {attributes.Count}");



                foreach (var attribute in attributes)
                {
                    Log($"--- {attribute.Name}");

                    var arguments = attribute
                        .DescendantNodes()
                        .OfType<AttributeArgumentSyntax>()
                        .ToList();

                    if (arguments.Count != 3)
                    {
                        Log($"--- skipping, has {arguments.Count} args");
                        continue;
                    }

                    var prop = AttributeUtils.PullArg(arguments[0])?.GetText().ToString();
                    var ident = AttributeUtils.PullArg(arguments[1])?.GetText().ToString();
                    var val = AttributeUtils.PullArg(arguments[2])?.GetText().ToString();

                    if (prop is null || ident is null || val is null)
                    {
                        Log("--- skipping, couldn't pull args");
                        continue;
                    }

                    if (!notifiersMap.TryGetValue(prop, out var map))
                        notifiersMap[prop] = map = new();

                    map.Add((ident, val));
                }
            }

            foreach (var notifier in notifiersMap)
            {
                Log(
                    $"[{notifier.Key}] {string.Join(", ", notifier.Value.Select(x => $"{x.Identifier} = {x.Value}"))}");



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
}
