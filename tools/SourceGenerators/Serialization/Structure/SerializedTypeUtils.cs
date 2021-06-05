using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Discord.Net.SourceGenerators.Serialization
{
    internal static class SerializedTypeUtils
    {
        public static List<SerializedType> BuildTypeTrees(
            INamedTypeSymbol generateSerializerAttribute,
            INamedTypeSymbol discriminatedUnionSymbol,
            INamedTypeSymbol discriminatedUnionMemberSymbol,
            IEnumerable<INamedTypeSymbol> symbolsToBuild)
        {
            var types = new List<SerializedType>();

            FindAllSerializedTypes(types, generateSerializerAttribute,
                discriminatedUnionSymbol, discriminatedUnionMemberSymbol,
                symbolsToBuild);

            // Now, move DU members into their relevant DU declaration.
            int x = 0;
            while (x < types.Count)
            {
                var type = types[x];
                if (type is DiscriminatedUnionMemberSerializedType duMember)
                {
                    var declaration = types.FirstOrDefault(
                        x => SymbolEqualityComparer.Default.Equals(
                            x.Declaration, duMember.Declaration.BaseType));

                    if (declaration is not DiscriminatedUnionSerializedType duDeclaration)
                        throw new InvalidOperationException(
                            "Could not find DU declaration for DU " +
                            $"member {duMember.Declaration.ToDisplayString()}");

                    duDeclaration.Members.Add(duMember with
                    {
                        DiscriminatedUnionDeclaration = duDeclaration
                    });
                    types.RemoveAt(x);
                    continue;
                }

                x++;
            }

            return types;
        }

        private static void FindAllSerializedTypes(
            List<SerializedType> types,
            INamedTypeSymbol generateSerializerAttribute,
            INamedTypeSymbol discriminatedUnionSymbol,
            INamedTypeSymbol discriminatedUnionMemberSymbol,
            IEnumerable<INamedTypeSymbol> symbolsToBuild)
        {
            foreach (var type in symbolsToBuild)
            {
                var generateSerializer = type.GetAttributes()
                    .Any(x => SymbolEqualityComparer.Default
                        .Equals(x.AttributeClass, generateSerializerAttribute));

                if (!generateSerializer)
                    continue;

                var duDeclaration = type.GetAttributes()
                    .FirstOrDefault(x => SymbolEqualityComparer.Default
                        .Equals(x.AttributeClass, discriminatedUnionSymbol));

                if (duDeclaration != null)
                {
                    if (duDeclaration
                        .ConstructorArguments
                        .FirstOrDefault()
                        .Value is not string memberName)
                        throw new InvalidOperationException(
                            "Failed to get DU discriminator member name");

                    var member = type.GetMembers(memberName)
                        .FirstOrDefault(
                            x => x is IPropertySymbol or IFieldSymbol);

                    if (member is null)
                        throw new InvalidOperationException(
                            "Failed to get DU discriminator member symbol");

                    types.Add(new DiscriminatedUnionSerializedType(
                        type, member));

                    continue;
                }

                var duMemberDeclaration = type
                    .GetAttributes()
                    .FirstOrDefault(x => SymbolEqualityComparer.Default
                        .Equals(x.AttributeClass,
                            discriminatedUnionMemberSymbol));

                if (duMemberDeclaration != null)
                {
                    if (duMemberDeclaration.ConstructorArguments.Length == 0
                        || duMemberDeclaration.ConstructorArguments[0].IsNull)
                        throw new InvalidOperationException(
                            "Failed to get DU discriminator value");

                    types.Add(new DiscriminatedUnionMemberSerializedType(
                        type, duMemberDeclaration.ConstructorArguments[0]));

                    continue;
                }

                types.Add(new SerializedType(type));
            }
        }
    }
}
