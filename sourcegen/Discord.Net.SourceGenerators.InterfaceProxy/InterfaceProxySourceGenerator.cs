using Discord.Net.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Discord.Net.SourceGenerators.InterfaceProxy;

[Generator]
public class InterfaceProxySourceGenerator : DiscordSourceGenerator
{
    public const string AttributeTarget = "ProxyInterface";

    public override void OnExecute(in GeneratorExecutionContext context)
    {
        var targets = Searching.FindPropertiesWithAttribute(in context, AttributeTarget);

        Log($"found {targets.Length} targets");

        foreach (var target in targets)
        {
            Log($"processing target {target.Identifier.ValueText}");

            var semanticTarget = context.Compilation.GetSemanticModel(target.SyntaxTree);

            var typeSymbol = semanticTarget.GetDeclaredSymbol(target);

            if (typeSymbol is null)
            {
                Log($"- Couldn't find type symbol for '{target.Identifier}'");
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
                Log($"--> Processing property {targetProperty.Identifier.ValueText}");

                // pull the attribute types
                var types = targetProperty.AttributeLists.SelectMany(x => x.Attributes)
                    .First(x => x.Name.GetText().ToString() == AttributeTarget)
                    .DescendantNodes()
                    .OfType<TypeOfExpressionSyntax>()
                    .ToList();

                Log($"--> found {types.Count} proxy type");

                foreach (var toProxy in types)
                {
                    Log($"---> processing proxy type {toProxy.Type}");

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
                        Log("---> No type info found");
                        continue;
                    }

                    var metadataName = semanticToProxy.Type.GetFullMetadataName();

                    Log($"---> Searching '{metadataName}'");

                    var proxyTypeSymbol = semanticToProxy.Type.ContainingAssembly.GetTypeByMetadataName(metadataName);

                    if (proxyTypeSymbol is null)
                    {
                        Log("---> No type symbol found");
                        continue;
                    }

                    var proxiedMembers = new List<string>();

                    var members = proxyTypeSymbol.GetMembers();

                    Log($"---> Members: {members.Length}");

                    foreach (var proxyMember in members)
                    {
                        switch (proxyMember)
                        {
                            case IMethodSymbol method:
                                if (method.IsStatic)
                                {
                                    Log($"----> Skipping {method.Name} (static)");
                                    continue;
                                }

                                if (method.MethodKind is not MethodKind.Ordinary)
                                {
                                    Log($"----> Skipping {method.Name} ({method.MethodKind})");
                                    continue;
                                }

                                var parameterIdentifiers = method.Parameters.Select(x => x.Name);
                                var parameterSymbols = method.Parameters.Select(x =>
                                {
                                    var param = $"{ResolveGeneric(x.Type, proxyTypeSymbol, semanticToProxy)} {x.Name}";


                                    if (x.HasExplicitDefaultValue)
                                        param += x.ExplicitDefaultValue is null
                                            ? " = default"
                                            : $" = {x.ExplicitDefaultValue.ToString().ToLowerInvariant()}";
                                    else if (x.IsOptional)
                                        param += " = default";

                                    return param;
                                });

                                Log($"----> Adding {method}");

                                proxiedMembers.Add(
                                    $"public {ResolveGeneric(method.ReturnType, proxyTypeSymbol, semanticToProxy)} {method.Name}({string.Join(", ", parameterSymbols)}) => ({targetProperty.Identifier} as {semanticToProxy.Type}).{method.Name}({string.Join(", ", parameterIdentifiers)});"
                                );
                                break;
                            case IPropertySymbol property:
                                if (property.ExplicitInterfaceImplementations.Length > 0)
                                {
                                    Log($"----> Skipping {property.Name} (has explicit impl)");
                                    continue;
                                }

                                Log($"----> Adding {property}");

                                proxiedMembers.Add(
                                    $"public {ResolveGeneric(property.Type, proxyTypeSymbol, semanticToProxy)} {property.Name} => ({targetProperty.Identifier} as {semanticToProxy.Type}).{property.Name};"
                                );
                                break;
                            default:
                                Log($"----> Unknown symbol '{proxyMember.GetType()}'");
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

    private ITypeSymbol ResolveGeneric(ITypeSymbol arg, INamedTypeSymbol root, TypeInfo info)
    {
        if (root.TypeArguments.Any(arg.Equals))
        {
            var typeArg = ((info.Type as INamedTypeSymbol)!).TypeArguments[root.TypeArguments.IndexOf(arg)];
            Log(
                $"-----> Generic found: {typeArg}");

            return typeArg.WithNullableAnnotation(arg.NullableAnnotation);
        }

        Log($"-----> Processing {arg}");

        if (arg is not INamedTypeSymbol namedArg)
            return arg;

        if (namedArg is {IsGenericType: false, IsUnboundGenericType: false})
            return arg;

        var newTypes = namedArg.TypeArguments
            .Select(x => ResolveGeneric(x, root, info)).ToArray();

        if (!newTypes.SequenceEqual(namedArg.TypeArguments, SymbolEqualityComparer.Default))
        {
            Log($"-----> {namedArg}");

            foreach (var newType in newTypes)
            {
                Log($"------> Constructing with {newType} {newType.BaseType}");
            }

            return namedArg.ConstructedFrom.Construct(newTypes);
        }

        return arg;
    }
}
