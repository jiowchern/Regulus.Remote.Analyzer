using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodParamsNoInterfaceAnalyzer : InterfaceSyntaxNodeAnalyzer
    {

        public MethodParamsNoInterfaceAnalyzer() : base( ERRORID.RRE2 , SyntaxKind.MethodDeclaration)
        {

        }

        public override bool NeedReport(SyntaxNodeAnalysisContext context, out Report report)
        {
            report = null;

            var methodNode = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax;
            var symbol = (IMethodSymbol)context.ContainingSymbol;

            foreach (var p in methodNode.ParameterList.Parameters.Zip(symbol.Parameters, (f, s) => new { Syntax = f, Symbol = s }))
            {
                if (p.Symbol.Type.TypeKind != TypeKind.Interface)
                {
                    continue;
                }
                

                report = new Report(p.Syntax.GetLocation(), p.Symbol.Name);
                return true;
            }
            return false;

        }
    }
}
