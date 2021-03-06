using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.NotFixableReturnTypeAnalyzer,
    Regulus.Remote.CodeAnalysis.NoFixedAnalysisCodeFixProvider>;

namespace Regulus.Remote.CodeAnalysis.Test
{
    [TestClass]
    public class NoFixedReturnValueCodeAnalysisUnitTests
    {
        [TestMethod]
        public async Task TestReturnTypeGeneric()
        {
            var test = @"
namespace ConsoleApplication1
{
    public class Foo<T>
    {}
    [Regulus.Remote.Attributes.SyntaxCheck()]
    public interface IFoo
    {       
        Foo<int> Method();        
    }
}
";
            var expected = VerifyCS.Diagnostic(NotFixableReturnTypeAnalyzer.DiagnosticId).WithSpan(9, 9, 9, 17).WithArguments("Foo");
            
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
