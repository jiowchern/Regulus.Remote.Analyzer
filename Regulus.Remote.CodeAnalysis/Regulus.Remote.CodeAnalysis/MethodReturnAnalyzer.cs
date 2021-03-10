using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace Regulus.Remote.CodeAnalysis
{

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodReturnAnalyzer : InterfaceSyntaxNodeAnalyzer
    {
        
        public MethodReturnAnalyzer() : base(ERRORID.RRE1)
        {
        }

        public override void Analysis(SyntaxNodeAnalysisContext context)
        {
            var symbol = (IMethodSymbol)context.ContainingSymbol;            

            var retType = symbol.ReturnType as INamedTypeSymbol;

            var valueType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Value`1");
            if (SymbolEqualityComparer.Default.Equals(retType.OriginalDefinition, valueType))
            {
                return;
            }

            var voidType = context.Compilation.GetTypeByMetadataName("System.Void");
            if (SymbolEqualityComparer.Default.Equals(retType.OriginalDefinition, voidType))
            {
                return;
            }

            var methodNode = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax;
            var diagnostic = Diagnostic.Create(ERRORID.RRE1.GetDiagnostic(), methodNode.ReturnType.GetLocation(), retType.Name);
            context.ReportDiagnostic(diagnostic);
        }
       

      
    }
}
