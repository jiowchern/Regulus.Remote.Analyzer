using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;

namespace Analyzers
{
    public class TestAttribute : System.Attribute
    {

    }
    public class can_compile_and_analyze
    {
        public static async Task<IEnumerable<Diagnostic>> CompileAndAnalyze(
            Project project,
            DiagnosticAnalyzer analyzer
        )
        {
            var compilation = await project.GetCompilationAsync();

            var compilationWithAnalyzer = compilation
                .WithAnalyzers(
                    ImmutableArray.Create(analyzer)
                );

            // does not work with await!
            return compilationWithAnalyzer
                .GetAnalyzerDiagnosticsAsync()
                .Result;
        }
        public static Project MakeProjectWith(string class_source)
        {
            var projectId = ProjectId.CreateNewId("test");
            var sln = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(
                    projectId: projectId,
                    name: "test",
                    assemblyName: "testAssembly",
                    language: LanguageNames.CSharp
                )
                .AddMetadataReference(projectId, CorlibReference)
                .AddMetadataReference(projectId, SystemRuntimeReference)
                .AddMetadataReference(projectId, NetStdReference)
                .AddMetadataReference(projectId, SystemCoreReference)
                .AddMetadataReference(projectId, CSharpSymbolsReference)
                .AddMetadataReference(projectId, CodeAnalysisReference)
                .AddMetadataReference(projectId, DataContractAttributeReference)
                .AddMetadataReference(projectId, RegulusRemoteReference);

            const string filename = "class_as_string.cs";
            var documentId = DocumentId.CreateNewId(projectId, filename);
            sln = sln.AddDocument(
                documentId,
                filename,
                SourceText.From(class_source)
            );
            
            return sln.GetProject(projectId);
        }

        static readonly MetadataReference CorlibReference =
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        static readonly MetadataReference SystemRuntimeReference =
            MetadataReference.CreateFromFile(typeof(GCSettings).Assembly.Location);
        static readonly MetadataReference NetStdReference =
            MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location);
        static readonly MetadataReference SystemCoreReference =
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        static readonly MetadataReference CSharpSymbolsReference =
            MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        static readonly MetadataReference CodeAnalysisReference =
            MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);
        static readonly MetadataReference RegulusRemoteReference =
            MetadataReference.CreateFromFile(typeof(Regulus.Remote.Syntax.CheckInterfaceAttribute).Assembly.Location);
        static readonly MetadataReference DataContractAttributeReference =
            MetadataReference.CreateFromFile(typeof(System.Runtime.Serialization.DataContractAttribute).Assembly.Location);
    }
}
