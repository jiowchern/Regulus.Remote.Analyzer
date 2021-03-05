using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

namespace Regulus.Remote.CodeAnalysis
{
    public static class SymbolExtensions
    {
        internal static bool ContainsAttributeType(this ImmutableArray<AttributeData> attributes, INamedTypeSymbol attributeType, bool exactMatch = false)
            => attributes.Any(a => attributeType.IsAssignableFrom(a.AttributeClass, exactMatch));

        internal static bool IsAssignableFrom(this INamedTypeSymbol targetType, INamedTypeSymbol sourceType, bool exactMatch = false)
        {

            if (targetType != null)
            {
                while (sourceType != null)
                {

                    if (SymbolEqualityComparer.Default.Equals(sourceType, targetType))
                        return true;

                    if (exactMatch)
                        return false;

                    if (targetType.TypeKind == TypeKind.Interface)
                        return sourceType.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, targetType));

                    sourceType = sourceType.BaseType;
                }
            }

            return false;
        }
   
    }
}
