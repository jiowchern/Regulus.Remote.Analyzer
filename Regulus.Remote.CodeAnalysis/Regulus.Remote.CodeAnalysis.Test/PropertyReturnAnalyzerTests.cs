using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

using VerifyCS = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.PropertyReturnAnalyzer,
    Regulus.Remote.CodeAnalysis.PropertyReturnAnalyzerCodeFixProvider>;

namespace Regulus.Remote.CodeAnalysis.Test
{
    [TestClass]
    public class PropertyReturnAnalyzerTests
    {
        [TestMethod]
        public async Task FixedReturnToPropertyTest()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        int Property1 { get; }
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
            var expected = VerifyCS.Diagnostic(PropertyReturnAnalyzer.DiagnosticId).WithSpan(7, 9, 7, 12).WithArguments("Int32");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixTest);
        }


        [TestMethod]
        public async Task Test()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        int Property1 { get; }
    }
}
";
  
            var expected = VerifyCS.Diagnostic(PropertyReturnAnalyzer.DiagnosticId).WithSpan(7, 9, 7, 12).WithArguments("int");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);            
        }

    }
}
