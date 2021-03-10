using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

using VerifyFixRemoveCS = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.PropertyAccessAnalyzer,
    Regulus.Remote.CodeAnalysis.NoFixedAnalysisCodeFixProvider>;

namespace Regulus.Remote.CodeAnalysis.Test
{
    [TestClass]
    public class PropertyAccessAnalyzerTests
    {
        [TestMethod]
        public async Task NoSetter()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        Regulus.Remote.Property<int> Property1 { get; set; }
    }
}
";
            var fixTest = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        Regulus.Remote.Property<int> Property1 { get; }
    }
}
";

            var expected = VerifyFixRemoveCS.Diagnostic(ERRORID.RRE6.GetDiagnosticId()).WithSpan(7, 55, 7, 59).WithArguments("set");
            //await VerifyFixRemoveCS.VerifyCodeFixAsync(test, expected, fixTest);
            await VerifyFixRemoveCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
