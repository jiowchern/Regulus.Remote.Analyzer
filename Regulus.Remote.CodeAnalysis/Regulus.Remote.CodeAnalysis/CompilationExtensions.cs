namespace Regulus.Remote.CodeAnalysis
{
    public static class CompilationExtensions
    {
        internal static Microsoft.CodeAnalysis.INamedTypeSymbol GetTypeBySystemType(this Microsoft.CodeAnalysis.Compilation compilation , System.Type type )
        {
            return compilation.GetTypeByMetadataName(type.FullName);
        }

        internal static Microsoft.CodeAnalysis.INamedTypeSymbol GetTypeBySystemType<T>(this Microsoft.CodeAnalysis.Compilation compilation)
        {
            return compilation.GetTypeByMetadataName(typeof(T).FullName);
        }
    }
}
