using System.Collections.Immutable;
using System.Text;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks;

public sealed class ContainerizedTrait : ISyntaxGenerationTask<ContainerizedTrait.GenerationTarget>
{
    public sealed class GenerationTarget(
        INamedTypeSymbol traitSelfTarget,
        INamedTypeSymbol symbol,
        ITypeSymbol idType,
        Compilation compilation
    ) : IEquatable<GenerationTarget>
    {
        public INamedTypeSymbol TraitSelfTarget { get; } = traitSelfTarget;
        public INamedTypeSymbol Symbol { get; } = symbol;
        public ITypeSymbol IdType { get; } = idType;
        public Compilation Compilation { get; } = compilation;

        private string Key
            => TraitSelfTarget.ToDisplayString()
               + string.Join("|", Symbol.AllInterfaces.Select(x => x.ToDisplayString()).OrderBy(x => x));

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Key.Equals(other.Key);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is GenerationTarget other && Equals(other);
        }

        public override int GetHashCode()
            => Key.GetHashCode();

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right)
        {
            return !Equals(left, right);
        }
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
        => node is InterfaceDeclarationSyntax iface && iface.Modifiers.IndexOf(SyntaxKind.PartialKeyword) != -1;

    public GenerationTarget? GetTargetForGeneration(
        GeneratorSyntaxContext context,
        Logger logger,
        CancellationToken token = default)
    {
        if (context.SemanticModel.Compilation.Assembly.Name != "Discord.Net.V4.Rest") return null;

        if (context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol symbol) return null;

        if (!symbol.GetAttributes()
                .Any(v => v.AttributeClass?.ToDisplayString() == "Discord.Rest.ContainerizedAttribute"))
            return null;
        
        var traitProvider = symbol.AllInterfaces.FirstOrDefault(x =>
            x is {Name: "IRestTraitProvider", TypeParameters.Length: 1}
        );

        if (traitProvider?.TypeArguments[0] is not INamedTypeSymbol selfTarget) return null;

        var identifiable = selfTarget.AllInterfaces
            .FirstOrDefault(x => x is {Name: "IIdentifiable", TypeArguments.Length: 1});

        if (identifiable is null) return null;
        
        return new GenerationTarget(
            selfTarget,
            symbol,
            identifiable.TypeArguments[0],
            context.SemanticModel.Compilation
        );
    }

    public void Execute(SourceProductionContext context, GenerationTarget? target, Logger logger)
    {
        if (target is null) return;

        var result =
            $$"""
              public partial interface {{target.Symbol.Name}}
              {
                  internal static readonly WeakDictionary<{{target.IdType}}, {{target.Symbol}}> _cache = new();
                  
                  internal static {{target.Symbol}} GetContainerized({{target.TraitSelfTarget}} target)
                  {
                      return _cache.GetOrAdd(target.Id, (actor, _) => new Container(target), target);
                  }
                  
                  {{GenerateContainerType(target, logger).Replace("\n", "\n    ")}}
              }
              """;

        foreach (var container in TypeUtils.ContaingTypes(target.Symbol))
        {
            result = $$"""
                       public partial interface {{container.Name}}
                       {
                          {{result.WithNewlinePadding(4)}}
                       }
                       """;
        }
        
        context.AddSource(
            $"TraitContainers/{target.Symbol.ToFullMetadataName()}",
            $$"""
            using Discord;
            using Discord.Rest;
            
            namespace {{target.Symbol.ContainingNamespace.ToDisplayString()}};
            
            {{result}}
            """
        );
    }

    private static string GenerateContainerType(GenerationTarget target, Logger logger)
    {
        return 
            $$"""
            private sealed class Container : {{target.Symbol.Name}}
            {
                private readonly {{target.TraitSelfTarget}} _target;
                
                public Container({{target.TraitSelfTarget}} target)
                {
                    _target = target;
                }
                
                {{GenerateContainerMembers(target, logger).Replace("\n", "\n    ")}}
            }   
            """;
    }

    private static string GenerateContainerMembers(
        GenerationTarget target,
        Logger logger)
    {
        var builder = new StringBuilder();
        
        var members = target.Symbol.AllInterfaces.Prepend(target.Symbol)
            .SelectMany(x => x.GetMembers())
            .Where(x => x switch
            {
                IPropertySymbol property => property is {IsAbstract: true, ExplicitInterfaceImplementations.Length: 0},
                IMethodSymbol method => method is {IsAbstract: true, ExplicitInterfaceImplementations.Length: 0, MethodKind: MethodKind.Ordinary},
                _ => false
            })
            .Distinct(SymbolEqualityComparer.Default);

        foreach (var member in members)
        {
            if(target.Symbol.FindImplementationForInterfaceMember(member) is not null) continue;
            
            if(target.Symbol.MemberNames.Contains(member.Name)) continue;

            var selfMembers = TypeUtils.GetBaseTypesAndThis(target.TraitSelfTarget)
                .SelectMany(x => x.GetMembers())
                .ToImmutableHashSet(SymbolEqualityComparer.Default);
            
            var impl = target.TraitSelfTarget
                .FindImplementationForInterfaceMember(member)
                ?? selfMembers.FirstOrDefault(x => x.Name == member.Name);
            
            var method = member as IMethodSymbol;
            
            if (impl is not null)
            {
                builder.Append(
                    $"{MemberUtils.GetMemberType(member)} {member.ContainingType}.{MemberUtils.GetMemberName(member)}"
                );

                if (method is not null)
                {
                    if (method.TypeArguments.Length > 0)
                        builder.Append($"<{string.Join(", ", method.TypeArguments.Select(x => x.Name))}>");

                    builder.Append($"({string.Join(", ", method.Parameters.Select(x => $"{x.Type} {x.Name}"))})");
                }

                builder.Append(" => ");
                
                if(target.Compilation.HasImplicitConversion(target.TraitSelfTarget, member.ContainingType))
                    builder.Append($"(_target as {member.ContainingType})");
                else
                    builder.Append($"({MemberUtils.GetMemberType(member)})_target");

                builder.Append($".{MemberUtils.GetMemberName(member)}");

                if (method is not null)
                {
                    builder.Append($"({string.Join(", ", method.Parameters.Select(x => x.Name))})");
                }

                builder.AppendLine(";");
            }
            else
            {
                logger.Warn($"{target.Symbol}: Unknown resolution for {member}");
                builder.AppendLine($"// {member}");
            }
        }

        return builder.ToString();
    }
    
    // internal static readonly WeakDictionary<ulong, IRestIntegrationChannelTrait> _cache = new();
    //
    // internal static IRestIntegrationChannelTrait GetContainerized(
    //     RestGuildChannelActor actor
    // ) => _cache.GetOrAdd(actor.Id, (actor, _) => new Container(actor), actor);
    //
    // private sealed class Container : IRestIntegrationChannelTrait
    // {
    //     private readonly RestGuildChannelActor _actor;
    //     
    //     public Container(RestGuildChannelActor actor)
    //     {
    //         _actor = actor;
    //     }
    //
    //     IGuildChannel IEntityProvider<IGuildChannel, IGuildChannelModel>.CreateEntity(IGuildChannelModel model)
    //         => _actor.CreateEntity(model);
    //
    //     IIntegrationChannel IIntegrationChannelTrait.CreateEntity(IGuildChannelModel model)
    //         => (IIntegrationChannel) _actor.CreateEntity(model);
    //
    //     IDiscordClient IClientProvider.Client => _actor.Client;
    //
    //     ulong IIdentifiable<ulong>.Id => _actor.Id;
    //
    //     IGuildActor IGuildRelationship.Guild => _actor.Guild;
    // }
}