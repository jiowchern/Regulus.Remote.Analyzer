using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PropertyReturnAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "rr0005";

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

            var attrs = symbol.ContainingSymbol.GetAttributes();
            var checkerType = context.Compilation.GetTypeBySystemType(typeof(Regulus.Remote.Attributes.SyntaxHelper));
            if (!attrs.ContainsAttributeType(checkerType))
                return;
            
            if (symbol.ContainingType.TypeKind != TypeKind.Interface)
                return;

            var retSymbol = symbol.Type as INamedTypeSymbol;
            
            var propertySymbol = context.Compilation.GetTypeBySystemType(typeof(Regulus.Remote.Property<>));
            if (SymbolEqualityComparer.Default.Equals(retSymbol.OriginalDefinition, propertySymbol))
            {
                return;
            }

            var notifierSymbol = context.Compilation.GetTypeBySystemType(typeof(Regulus.Remote.Notifier<>));
            if (SymbolEqualityComparer.Default.Equals(retSymbol.OriginalDefinition, notifierSymbol))
            {
                return;
            }

            
            var diagnostic = Diagnostic.Create(_DiagnosticDescriptor, context.Node.GetLocation() , retSymbol.Name);
            context.ReportDiagnostic(diagnostic);

        }
    }
}
