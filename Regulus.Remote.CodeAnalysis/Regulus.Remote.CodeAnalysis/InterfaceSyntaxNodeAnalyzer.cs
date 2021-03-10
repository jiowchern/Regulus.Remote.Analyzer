using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public abstract class InterfaceSyntaxNodeAnalyzer : DiagnosticAnalyzer
    {
        public readonly DiagnosticDescriptor Descriptor;        

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        public InterfaceSyntaxNodeAnalyzer(ERRORID id )
        {
            Descriptor = DiagnosticDescriptorProvider.Instance[id];            
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_Analysis, SyntaxKind.MethodDeclaration , SyntaxKind.EventDeclaration , SyntaxKind.PropertyDeclaration );
        }
        public abstract void Analysis(SyntaxNodeAnalysisContext context);
        private void _Analysis(SyntaxNodeAnalysisContext context)
        {
            if(_NoHelp(context))            
                return;
            
            if (_NoInterface(context))
                return;

            Analysis(context);
        }

        private bool _NoInterface(SyntaxNodeAnalysisContext context)
        {
            return context.ContainingSymbol.ContainingType.TypeKind != TypeKind.Interface;                
        }

        private bool _NoHelp(SyntaxNodeAnalysisContext context)
        {
            var parentSymbol = context.ContainingSymbol.ContainingType;
            var helperType = context.Compilation.GetTypeBySystemType(typeof(Regulus.Remote.Attributes.SyntaxHelper));
            return !parentSymbol.GetAttributes().ContainsAttributeType(helperType);
        }
    }
}
