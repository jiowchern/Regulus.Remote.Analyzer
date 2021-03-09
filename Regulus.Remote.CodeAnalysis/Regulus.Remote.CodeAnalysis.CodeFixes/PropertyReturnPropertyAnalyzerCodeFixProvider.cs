using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Regulus.Remote.CodeAnalysis
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyReturnNotifierAnalyzerCodeFixProvider)), Shared]
    public class PropertyReturnNotifierAnalyzerCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PropertyReturnAnalyzer.DiagnosticId);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var node = root.FindNode(diagnosticSpan);

            context.RegisterCodeFix(
                Microsoft.CodeAnalysis.CodeActions.CodeAction.Create(
                    title: CodeFixResources.rr0005TitleNotifier,
                    createChangedSolution: c => _MakeNotifier(context.Document, node as Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax, c),
                    equivalenceKey: nameof(CodeFixResources.rr0005TitleNotifier)),
                diagnostic);





        }

        private async Task<Solution> _MakeNotifier(Document document, TypeSyntax type_syntax, CancellationToken c)
        {
            var root = await document.GetSyntaxRootAsync(c).ConfigureAwait(false);

            var newNode = SyntaxFactory.ParseTypeName($"Regulus.Remote.Notifier<{type_syntax}> ");

            var newRoot = root.ReplaceNode(type_syntax, newNode);
            return document.Project.Solution.WithDocumentSyntaxRoot(document.Id, newRoot);
        }
        

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }
    }
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyReturnPropertyAnalyzerCodeFixProvider)), Shared]
    public class PropertyReturnPropertyAnalyzerCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PropertyReturnAnalyzer.DiagnosticId);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var node = root.FindNode(diagnosticSpan);

            

            context.RegisterCodeFix(
                Microsoft.CodeAnalysis.CodeActions.CodeAction.Create(
                    title: CodeFixResources.rr0005TitleProperty,
                    createChangedSolution: c => _MakeProperty(context.Document, node as Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax, c),
                    equivalenceKey: nameof(CodeFixResources.rr0005TitleProperty)),
                diagnostic);

            

        }

        

        private async Task<Solution> _MakeProperty(Document document, TypeSyntax type_syntax, CancellationToken c)
        {
            var root = await document.GetSyntaxRootAsync(c).ConfigureAwait(false);
            
            var newNode = SyntaxFactory.ParseTypeName($"Regulus.Remote.Property<{type_syntax}> ");            

            var newRoot = root.ReplaceNode(type_syntax, newNode);
            return document.Project.Solution.WithDocumentSyntaxRoot(document.Id, newRoot);
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }
    }
}
