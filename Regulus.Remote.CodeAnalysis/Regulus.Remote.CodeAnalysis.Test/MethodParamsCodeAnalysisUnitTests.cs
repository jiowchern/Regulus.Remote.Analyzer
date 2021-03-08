using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.MethodParamsTypeAnalyzer,
    Regulus.Remote.CodeAnalysis.NoFixedAnalysisCodeFixProvider>;

namespace Regulus.Remote.CodeAnalysis.Test
{
    
    [TestClass]
    public class MethodParamsCodeAnalysisUnitTests
    {
        [TestMethod]
        public async Task InterfaceTest()
        {
            var test = @"
namespace ConsoleApplication1
{
    public interface IParam
    {
    }
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        void Method(IParam p1);        
    }
}
";
            var expected = VerifyCS.Diagnostic(MethodParamsTypeAnalyzer.DiagnosticInterfaceId).WithSpan(10, 21, 10, 30).WithArguments("p1");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task GenericTest()
        {
            var test = @"
namespace ConsoleApplication1
{
    public class Param<T>
    {
    }
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        void Method(Param<int> p1);        
    }
}
";
            var expected = VerifyCS.Diagnostic(MethodParamsTypeAnalyzer.DiagnosticGenericId).WithSpan(10, 21, 10, 34).WithArguments("p1");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
