using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyNoGenericTypeCS = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.MethodParamsNoGenericTypeAnalyzer,
    Regulus.Remote.CodeAnalysis.NoFixedAnalysisCodeFixProvider>;

using VerifyNoInterfaceCS = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.MethodParamsNoInterfaceAnalyzer,
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
            var expected = VerifyNoInterfaceCS.Diagnostic(ERRORID.RRE2.GetDiagnostic()).WithSpan(10, 21, 10, 30).WithArguments("p1");

            await VerifyNoInterfaceCS.VerifyAnalyzerAsync(test, expected);
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
            var expected = VerifyNoGenericTypeCS.Diagnostic(ERRORID.RRE3.GetDiagnostic()).WithSpan(10, 21, 10, 34).WithArguments("p1");

            await VerifyNoGenericTypeCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
