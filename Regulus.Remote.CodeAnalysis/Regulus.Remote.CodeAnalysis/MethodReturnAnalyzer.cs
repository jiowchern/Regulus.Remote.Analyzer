using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace Regulus.Remote.CodeAnalysis
{

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodReturnAnalyzer : InterfaceSyntaxNodeAnalyzer
    {
        
        public MethodReturnAnalyzer() : base(ERRORID.RRE1 , Microsoft.CodeAnalysis.CSharp.SyntaxKind.MethodDeclaration)
        {
        }

        public override bool NeedReport(SyntaxNodeAnalysisContext context , out InterfaceSyntaxNodeAnalyzer.Report report )
        {
            report = null;
            if (context.ContainingSymbol.Kind != SymbolKind.Method)
                return false;

            var symbol = (IMethodSymbol)context.ContainingSymbol;            

            var retType = symbol.ReturnType as INamedTypeSymbol;

            var valueType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Value`1");
            if (SymbolEqualityComparer.Default.Equals(retType.OriginalDefinition, valueType))
            {
                return false;
            }

            var voidType = context.Compilation.GetTypeByMetadataName("System.Void");
            if (SymbolEqualityComparer.Default.Equals(retType.OriginalDefinition, voidType))
            {
                return false;
            }
            var methodNode = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax;
            report = new Report(methodNode.ReturnType.GetLocation(), retType.Name);
            return true;
        }
       

      
    }
}
