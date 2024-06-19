using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Discord.Net.SourceGenerators.InterfaceProxy;

[Generator]
public class InterfaceProxySourceGenerator : ISourceGenerator
{
    public const string AttributeTarget = "ProxyInterface";

    public void Initialize(GeneratorInitializationContext context) {}

    public void Execute(GeneratorExecutionContext context)
    {
        var log = new StringBuilder();

        try
        {
            var targets = FindTargets(ref context);

            log.AppendLine($"// found {targets.Length} targets");

            foreach (var target in targets)
            {
                log.AppendLine($"// processing target {target.Identifier.ValueText}");

                var semanticTarget = context.Compilation.GetSemanticModel(target.SyntaxTree);

                var typeSymbol = semanticTarget.GetDeclaredSymbol(target);

                if (typeSymbol is null)
                {
                    log.AppendLine($"// - Couldn't find type symbol for '{target.Identifier}'");
                    continue;
                }

                // get the property with the attribute
                var targetProperties = target.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .Where(x => x.AttributeLists
                        .SelectMany(x => x.Attributes)
                        .Any(x => x.Name.GetText().ToString() == AttributeTarget)
                    );

                foreach (var targetProperty in targetProperties)
                {
                    log.AppendLine($"// --> Processing property {targetProperty.Identifier.ValueText}");

                    // pull the attribute types
                    var types = targetProperty.AttributeLists.SelectMany(x => x.Attributes)
                        .First(x => x.Name.GetText().ToString() == AttributeTarget)
                        .DescendantNodes()
                        .OfType<TypeOfExpressionSyntax>()
                        .ToList();

                    log.AppendLine($"// --> found {types.Count} proxy type");

                    foreach (var toProxy in types)
                    {
                        log.AppendLine($"// ---> processing proxy type {toProxy.Type}");

                        TypeInfo semanticToProxy;

                        if (toProxy.DescendantNodes().FirstOrDefault() is GenericNameSyntax generic)
                        {
                            semanticToProxy = semanticTarget.GetTypeInfo(generic);
                        }
                        else
                        {
                            semanticToProxy = semanticTarget.GetTypeInfo(toProxy.Type);
                        }

                        if (semanticToProxy.Type is null)
                        {
                            log.AppendLine("// ---> No type info found");
                            continue;
                        }

                        var metadataName = GetFullMetadataName(semanticToProxy.Type);

                        log.AppendLine($"// ---> Searching '{metadataName}'");

                        var proxyTypeSymbol = semanticToProxy.Type.ContainingAssembly.GetTypeByMetadataName(metadataName);

                        if (proxyTypeSymbol is null)
                        {
                            log.AppendLine("// ---> No type symbol found");
                            continue;
                        }

                        var proxiedMembers = new List<string>();

                        var members = proxyTypeSymbol.GetMembers();

                        log.AppendLine($"// ---> Members: {members.Length}");

                        foreach (var proxyMember in members)
                        {
                            switch (proxyMember)
                            {
                                case IMethodSymbol method:
                                    if (method.IsStatic)
                                    {
                                        log.AppendLine($"// ----> Skipping {method.Name} (static)");
                                        continue;
                                    }

                                    if (method.MethodKind is not MethodKind.Ordinary)
                                    {
                                        log.AppendLine($"// ----> Skipping {method.Name} ({method.MethodKind})");
                                        continue;
                                    }

                                    var parameterIdentifiers = method.Parameters.Select(x => x.Name);
                                    var parameterSymbols = method.Parameters.Select(x =>
                                    {
                                        var param = $"{ResolveGeneric(x.Type, proxyTypeSymbol, semanticToProxy, log)} {x.Name}";


                                        if (x.HasExplicitDefaultValue)
                                            param += x.ExplicitDefaultValue is null ? " = default" : $" = {x.ExplicitDefaultValue.ToString().ToLowerInvariant()}";
                                        else if (x.IsOptional)
                                            param += " = default";

                                        return param;
                                    });

                                    log.AppendLine($"// ----> Adding {method}");

                                    proxiedMembers.Add(
                                        $"public {ResolveGeneric(method.ReturnType, proxyTypeSymbol, semanticToProxy, log)} {method.Name}({string.Join(", ", parameterSymbols)}) => ({targetProperty.Identifier} as {semanticToProxy.Type}).{method.Name}({string.Join(", ", parameterIdentifiers)});"
                                    );
                                    break;
                                case IPropertySymbol property:
                                    if (property.ExplicitInterfaceImplementations.Length > 0)
                                    {
                                        log.AppendLine($"// ----> Skipping {property.Name} (has explicit impl)");
                                        continue;
                                    }

                                    log.AppendLine($"// ----> Adding {property}");

                                    proxiedMembers.Add(
                                        $"public {ResolveGeneric(property.Type, proxyTypeSymbol, semanticToProxy, log)} {property.Name} => ({targetProperty.Identifier} as {semanticToProxy.Type}).{property.Name};"
                                    );
                                    break;
                                default:
                                    log.AppendLine($"// ----> Unknown symbol '{proxyMember.GetType()}'");
                                    break;
                            }
                        }

                        context.AddSource(
                            $"{target.Identifier}_{semanticToProxy.Type.Name}.g.cs",
                            $$"""
                            namespace {{typeSymbol.ContainingNamespace}};

                            #nullable enable

                            public partial class {{target.Identifier}}
                            {
                                {{string.Join("\n    ", proxiedMembers)}}
                            }

                            #nullable restore

                            """
                        );
                    }
                }
            }
        }
        finally
        {
            context.AddSource("log.g.cs", log.ToString());
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

    private static ClassDeclarationSyntax[] FindTargets(ref GeneratorExecutionContext context)
    {
        return context.Compilation.SyntaxTrees
            .SelectMany(x => x
                .GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(x => x
                    .DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>()
                    .Any(x => x.AttributeLists
                        .SelectMany(x => x.Attributes)
                        .Any(x => x.Name.GetText().ToString() == AttributeTarget))
                )
            ).ToArray();
    }

    public static string GetFullMetadataName(ISymbol? s)
    {
        if (s == null || IsRootNamespace(s))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(s.MetadataName);
        var last = s;

        s = s.ContainingSymbol;

        while (!IsRootNamespace(s))
        {
            if (s is ITypeSymbol && last is ITypeSymbol)
            {
                sb.Insert(0, '+');
            }
            else
            {
                sb.Insert(0, '.');
            }

            sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            //sb.Insert(0, s.MetadataName);
            s = s.ContainingSymbol;
        }

        return sb.ToString();
    }

    private static bool IsRootNamespace(ISymbol symbol)
    {
        INamespaceSymbol? s = null;
        return ((s = symbol as INamespaceSymbol) != null) && s.IsGlobalNamespace;
    }
}
