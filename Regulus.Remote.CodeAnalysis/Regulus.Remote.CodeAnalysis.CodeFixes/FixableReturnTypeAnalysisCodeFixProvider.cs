using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Regulus.Remote.CodeAnalysis
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FixableReturnTypeAnalysisCodeFixProvider)), Shared]
    public class FixableReturnTypeAnalysisCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(FixableReturnTypeAnalyzer.DiagnosticId); }
        }
        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindNode(diagnosticSpan);
            

            context.RegisterCodeFix(
                Microsoft.CodeAnalysis.CodeActions.CodeAction.Create(
                    title: CodeFixResources.ReturnValueCodeFixTitle,
                    createChangedSolution: c => _MakeRemoteValue(context.Document, node as TypeSyntax, c),
                    equivalenceKey: nameof(CodeFixResources.ReturnValueCodeFixTitle)),
                diagnostic);


        }

        private async Task<Solution> _MakeRemoteValue(Document document, TypeSyntax declaration, CancellationToken c)
        {

            var root = await document.GetSyntaxRootAsync(c).ConfigureAwait(false);
            //var newNode = SyntaxFactory.GenericName($"Regulus.Remote.Value<{declaration}>");
            var newNode = SyntaxFactory.ParseTypeName($"Regulus.Remote.Value<{declaration}> ");
            //var newNode = TypeSyntaxFactory.GetTypeSyntax("Regulus.Remote.Value", declaration );

            var newRoot = root.ReplaceNode(declaration, newNode );
            return document.Project.Solution.WithDocumentSyntaxRoot(document.Id, newRoot);
        }

     


    }
}
