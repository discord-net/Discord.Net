using Discord.Net.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Discord.Net.SourceGenerators.EntityHierarchies;

[Generator]
public class EntityHierarchiesSourceGenerator : ISourceGenerator
{
    public const string ExtendingAttributeName = "ExtendInterfaceDefaults";

    public void Initialize(GeneratorInitializationContext context)
    {

    }

    public void Execute(GeneratorExecutionContext context)
    {
        StringBuilder log = new();

        try
        {
            var targets = Searching.FindClassesWithAttribute(ref context, ExtendingAttributeName);
            var interfaces = GetInterfaces(ref context);

            log.AppendLine($"// {targets.Length} possible targets found");
            log.AppendLine($"// {interfaces.Length} interfaces declared");

            foreach (var target in targets)
            {
                log.AppendLine($"// Processing target {target.Identifier}");

                var semanticTarget = context.Compilation.GetSemanticModel(target.SyntaxTree);

                var attributes = target
                    .DescendantNodes()
                    .OfType<AttributeSyntax>()
                    .ToArray();

                log.AppendLine($"// - Attributes: {attributes.Length}:");

                // parameter identifiers of the attribute
                var nodes = attributes
                    .FirstOrDefault(a => a.DescendantNodes().OfType<IdentifierNameSyntax>()
                        .Any(dt => dt.Identifier.ValueText == ExtendingAttributeName))
                    ?.DescendantNodes().OfType<TypeOfExpressionSyntax>()
                    .ToList();

                if (nodes is null || nodes.Count == 0)
                {
                    log.AppendLine("// - no nodes found");
                    continue;
                }

                foreach (var node in nodes)
                {
                    log.AppendLine($"// - Processing node {node}");

                    TypeInfo semanticInterface;

                    if (node.DescendantNodes().FirstOrDefault() is GenericNameSyntax generic)
                    {
                        semanticInterface = semanticTarget.GetTypeInfo(generic);
                    }
                    else
                    {
                        semanticInterface = semanticTarget.GetTypeInfo(node.Type);
                    }

                    if (semanticInterface.Type is null)
                    {
                        log.AppendLine("// - No semantic type found for node");
                        continue;
                    }

                    log.AppendLine($"// -> {semanticInterface.Type} {semanticInterface.Type.ContainingAssembly}");

                    List<string> declarations;

                    var ifaceSyntax = interfaces
                        .FirstOrDefault(x => x.Identifier.ValueText == semanticInterface.Type.Name);


                    if (ifaceSyntax is not null)
                    {
                        declarations = CreateDeclarationsFromSource(ifaceSyntax, semanticInterface);
                    }
                    else
                    {
                        var metadataName = semanticInterface.Type.GetFullMetadataName();
                        var ifaceSymbol = semanticInterface.Type.ContainingAssembly.GetTypeByMetadataName(metadataName);

                        if (ifaceSymbol is null)
                        {
                            log.AppendLine("// - Couldn't find interface symbol");
                            continue;
                        }

                        declarations = CreateDeclarationsFromSymbol(ifaceSymbol, semanticInterface, log);
                    }

                    if (declarations.Count == 0)
                    {
                        log.AppendLine("// - no implementable declarations found, skipping");
                        continue;
                    }

                    foreach (var dec in declarations)
                    {
                        log.AppendLine($"// --> {dec}");
                    }

                    context.AddSource(
                        $"{target.Identifier.ValueText}_{semanticInterface.Type.Name}.g.cs",
                        $$"""
                          namespace {{semanticTarget.GetDeclaredSymbol(target)!.ContainingNamespace}};

                          #nullable enable

                          public partial class {{target.Identifier}}
                          {
                              {{string.Join("\n    ", declarations)}}
                          }

                          #nullable restore
                          """
                    );
                }
            }
        }
        catch (Exception x)
        {
            log.AppendLine(x.ToString());
        }
        finally
        {
            context.AddSource("Log.g.cs", log.ToString());
        }
    }

    private ITypeSymbol ResolveGeneric(ITypeSymbol arg, INamedTypeSymbol root, TypeInfo info, StringBuilder log)
    {
        if (root.TypeArguments.Any(arg.Equals))
        {
            var typeArg = ((info.Type as INamedTypeSymbol)!).TypeArguments[root.TypeArguments.IndexOf(arg)];
            log.AppendLine(
                $"// -----> Generic found: {typeArg}");

            return typeArg.WithNullableAnnotation(arg.NullableAnnotation);
        }

        log.AppendLine($"// -----> Processing {arg}");

        if (arg is not INamedTypeSymbol namedArg)
            return arg;

        if (namedArg is {IsGenericType: false, IsUnboundGenericType: false})
            return arg;

        var newTypes = namedArg.TypeArguments
            .Select(x => ResolveGeneric(x, root, info, log)).ToArray();

        if (!newTypes.SequenceEqual(namedArg.TypeArguments, SymbolEqualityComparer.Default))
        {
            log.AppendLine($"// -----> {namedArg}");

            foreach (var newType in newTypes)
            {
                log.AppendLine($"// ------> Constructing with {newType} {newType.BaseType}");
            }

            return namedArg.ConstructedFrom.Construct(newTypes);
        }

        return arg;
    }

    private List<string> CreateDeclarationsFromSymbol(INamedTypeSymbol symbol, TypeInfo semanticInterface, StringBuilder log)
    {
        var declarations = new List<string>();

        // methods are virtual
        foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>().Where(x => x.IsVirtual))
        {
            var parameterIdentifiers = string.Join(
                ", ",
                method.Parameters.Select(x => x.Name)
            );

            var parameterSymbols = method.Parameters.Select(x =>
            {
                var param = $"{ResolveGeneric(x.Type, symbol, semanticInterface, log)} {x.Name}";


                if (x.HasExplicitDefaultValue)
                    param += x.ExplicitDefaultValue is null ? " = default" : $" = {x.ExplicitDefaultValue.ToString().ToLowerInvariant()}";
                else if (x.IsOptional)
                    param += " = default";

                return param;
            });

            declarations.Add(
                $"public {ResolveGeneric(method.ReturnType, symbol, semanticInterface, log)} {method.Name}({string.Join(", ", parameterSymbols)})" +
                $" => (this as {semanticInterface.Type!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}).{method.Name}({parameterIdentifiers});"
            );
        }

        return declarations;
    }

    private List<string> CreateDeclarationsFromSource(InterfaceDeclarationSyntax iface, TypeInfo semanticInterface)
    {
        var declarations = new List<string>();

        foreach (var method in iface.Members.Cast<MethodDeclarationSyntax>())
        {
            var parameterIdentifiers = string.Join(
                ", ",
                method.ParameterList.Parameters.Select(x => x.Identifier.ValueText)
            );

            declarations.Add(
                $"public {method.ReturnType} {method.Identifier}{method.ParameterList}" +
                $" => (this as {semanticInterface.Type!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}).{method.Identifier}({parameterIdentifiers})"
            );
        }

        return declarations;
    }

    private InterfaceDeclarationSyntax[] GetInterfaces(ref GeneratorExecutionContext context)
        => context.Compilation.SyntaxTrees.SelectMany(x =>
            x.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>()
        ).ToArray();
}
