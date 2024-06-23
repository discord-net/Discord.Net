using Discord.Net.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discord.Net.SourceGenerators.PropertyVersioning;

[Generator]
public class PropertyVersioningSourceGenerator : DiscordSourceGenerator
{
    public const string VersionOnAttributeName = "VersionOn";

    public override void OnExecute(in GeneratorExecutionContext context)
    {
        var targets = Searching.FindPropertiesWithAttribute(in context, VersionOnAttributeName);

        Log($"Found {targets.Length} targets");

        foreach (var target in targets)
        {
            Log($" - Processing {target.Identifier}");

            var semanticTarget = context.Compilation.GetSemanticModel(target.SyntaxTree);
            var typeSymbol = semanticTarget.GetDeclaredSymbol(target);

            if (typeSymbol is null)
            {
                Log(" - No semantic type symbol found, skipping");
                continue;
            }

            var props = target.DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Select(x => (Property: x, Attribute: x.AttributeLists
                    .SelectMany(x => x.Attributes)
                    .FirstOrDefault(x => x.Name.GetText().ToString() == VersionOnAttributeName))
                )
                .Where(x => x.Item2 is not null)
                .ToArray();

            Log($" - Found {props.Length} properties");

            var generatedVersioning = new List<VersionProperty>();

            foreach (var (property, attribute) in props)
            {
                Log($" -- Processing {property.Identifier}");

                var arguments = attribute!
                    .DescendantNodes()
                    .OfType<AttributeArgumentSyntax>()
                    .ToList();

                var versionSource = AttributeUtils.PullArg(arguments[0]);
                var initSource = AttributeUtils.PullArg(arguments[1]);

                if (versionSource is null)
                {
                    Log(" --- Skipping: no version source parameter");
                    continue;
                }

                var field = $"private int? __{property.Identifier}Version";

                if (initSource is not null)
                {
                    field += $" = {initSource}?.GetHashCode();";
                }
                else
                {
                    field += ";";
                }

                var prop =
                    $"private bool Is{property.Identifier}OutOfDate => __{property.Identifier}Version != {versionSource}?.GetHashCode();";

                var onChanged =
                    $"private void On{property.Identifier}Changed() => __{property.Identifier}Version = {versionSource}?.GetHashCode();";

                generatedVersioning.Add(new VersionProperty(field, prop, onChanged));
            }

            Log($" - Generating {generatedVersioning.Count} versioned properties");

            context.AddSource(
                $"{target.Identifier}_Versioned",
                $$"""
                namespace {{typeSymbol.ContainingNamespace}};

                public partial class {{target.Identifier}}
                {
                    {{string.Join("\n\n    ", generatedVersioning.Select(x => $"{x.Field}\n    {x.IsOutOfDateProperty}\n    {x.OnChanged}"))}}
                }
                """
            );
        }
    }

    private readonly struct VersionProperty(string field, string isOutOfDateProperty, string onChanged)
    {
        public readonly string Field = field;
        public readonly string IsOutOfDateProperty = isOutOfDateProperty;
        public readonly string OnChanged = onChanged;
    }
}
