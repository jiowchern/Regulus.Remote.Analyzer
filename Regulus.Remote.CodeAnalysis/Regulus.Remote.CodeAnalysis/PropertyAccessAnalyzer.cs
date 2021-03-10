using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PropertyAccessAnalyzer : InterfaceSyntaxNodeAnalyzer
    {
        

        public PropertyAccessAnalyzer() : base(ERRORID.RRE6, Microsoft.CodeAnalysis.CSharp.SyntaxKind.PropertyDeclaration)
        {
        }

        
        public override bool NeedReport(SyntaxNodeAnalysisContext context, out Report report)
        {
            report = null;
            var symbol = context.ContainingSymbol as IPropertySymbol;
            var syntax = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax;

            
            var setterSyntax = syntax.AccessorList.Accessors.FirstOrDefault(a => a.Kind() == Microsoft.CodeAnalysis.CSharp.SyntaxKind.SetAccessorDeclaration);


            if (setterSyntax == null)
                return false;

            report = new Report(setterSyntax.GetLocation(), setterSyntax.Keyword.ValueText);
            return true;
            
        }

        
    }
}
