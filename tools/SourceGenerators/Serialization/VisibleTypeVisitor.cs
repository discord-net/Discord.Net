using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Discord.Net.SourceGenerators.Serialization
{
    internal sealed class VisibleTypeVisitor
        : SymbolVisitor
    {
        private readonly CancellationToken _cancellationToken;
        private readonly HashSet<INamedTypeSymbol> _typeSymbols;

        public VisibleTypeVisitor(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _typeSymbols = new(SymbolEqualityComparer.Default);
        }

        public IEnumerable<INamedTypeSymbol> GetVisibleTypes()
            => _typeSymbols;

        public override void VisitAssembly(IAssemblySymbol symbol)
        {
            _cancellationToken.ThrowIfCancellationRequested();
            symbol.GlobalNamespace.Accept(this);
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (var member in symbol.GetMembers())
            {
                _cancellationToken.ThrowIfCancellationRequested();
                member.Accept(this);
            }
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            var isVisible = symbol.DeclaredAccessibility switch
            {
                Accessibility.Protected => true,
                Accessibility.ProtectedOrInternal => true,
                Accessibility.Public => true,
                _ => false,
            };

            if (!isVisible || !_typeSymbols.Add(symbol))
                return;

            foreach (var member in symbol.GetTypeMembers())
            {
                _cancellationToken.ThrowIfCancellationRequested();
                member.Accept(this);
            }
        }
    }
}
