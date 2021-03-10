using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.MethodParamsCountAnalyzer,
    Regulus.Remote.CodeAnalysis.NoFixedAnalysisCodeFixProvider>;

namespace Regulus.Remote.CodeAnalysis.Test
{
    [TestClass]
    public class MethodParamsCountTypeAnalyzerTests
    {

        [TestMethod]
        public async Task Test()
        {
            var test = @"
namespace ConsoleApplication1
{
    
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        void Method(int p1,int p2,int p3,int p4,int p5,int p6);        
    }
}
";
            var expected = VerifyCS.Diagnostic(ERRORID.RRE4.GetDiagnostic()).WithSpan(8, 56, 8, 62).WithArguments("p6");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
