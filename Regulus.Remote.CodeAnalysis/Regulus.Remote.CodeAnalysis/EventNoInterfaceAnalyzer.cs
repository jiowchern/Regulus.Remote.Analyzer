using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EventNoInterfaceAnalyzer : InterfaceSyntaxNodeAnalyzer
    {
        public EventNoInterfaceAnalyzer() : base(ERRORID.RRE9, SyntaxKind.EventFieldDeclaration)
        {
        }

        public override bool NeedReport(SyntaxNodeAnalysisContext context, out Report report)
        {
            
            report = null;
            var symbol = context.ContainingSymbol as IEventSymbol;
            var typeSymbol = symbol.Type as INamedTypeSymbol;
            
            for (var i = 0; i < typeSymbol.TypeArguments.Length; ++i)
            {
                var item = typeSymbol.TypeArguments[i];
                if (item.TypeKind != TypeKind.Interface )
                {
                    continue;
                }
                var eventSyntax = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.EventFieldDeclarationSyntax;
                return false;
            }
            return false;
        }
    }
}
