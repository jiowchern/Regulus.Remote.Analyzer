using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Collections.Immutable;

namespace Regulus.Remote.CodeAnalysis.Test
{
    public static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        public class Test : CSharpCodeFixTest<TAnalyzer, TCodeFix, MSTestVerifier>
        {
            private ImmutableArray<string> _BuildAsms;

            public Test()
            {                
                SolutionTransforms.Add((solution, projectId) =>
                {
                    solution = solution.AddMetadataReference(projectId , MetadataReference.CreateFromFile(typeof(Regulus.Remote.Attributes.SyntaxCheck).Assembly.Location));
                    solution = solution.AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(Regulus.Remote.Value<>).Assembly.Location));

                    var compilationOptions = solution.GetProject(projectId).CompilationOptions;
                    compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                        compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
                    solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                    return solution;
                });
                
            }
        }
    }
}
