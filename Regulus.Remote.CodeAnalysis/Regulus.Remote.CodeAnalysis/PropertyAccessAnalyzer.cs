using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PropertyAccessAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "rr0006";

        static readonly DiagnosticDescriptor _DiagnosticDescriptor = new DiagnosticDescriptorCreateor(DiagnosticId).DiagnosticDescriptor;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_DiagnosticDescriptor);        

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_Analysis, Microsoft.CodeAnalysis.CSharp.SyntaxKind.PropertyDeclaration);


        }

        private void _Analysis(SyntaxNodeAnalysisContext context)
        {
            var symbol = context.ContainingSymbol as IPropertySymbol;
            var syntax = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax;

            var attrs = symbol.ContainingSymbol.GetAttributes();
            var checkerType = context.Compilation.GetTypeBySystemType(typeof(Regulus.Remote.Attributes.SyntaxHelper));
            if (!attrs.ContainsAttributeType(checkerType))
                return;

            if (symbol.ContainingType.TypeKind != TypeKind.Interface)
                return;
            var setterSyntax = syntax.AccessorList.Accessors.FirstOrDefault(a => a.Kind() == Microsoft.CodeAnalysis.CSharp.SyntaxKind.SetAccessorDeclaration);

            
            if (setterSyntax == null)
                return;

            
            var diagnostic = Diagnostic.Create(_DiagnosticDescriptor, setterSyntax.GetLocation() , setterSyntax.Keyword.ValueText);
            context.ReportDiagnostic(diagnostic);

        }
    }
}
