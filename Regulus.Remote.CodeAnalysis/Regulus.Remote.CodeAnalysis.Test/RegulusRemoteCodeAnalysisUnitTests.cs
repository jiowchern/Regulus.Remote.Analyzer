using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.RegulusRemoteCodeAnalysisAnalyzer,
    Regulus.Remote.CodeAnalysis.RegulusRemoteCodeAnalysisCodeFixProvider>;

namespace Regulus.Remote.CodeAnalysis.Test
{
    [TestClass]
    public class RegulusRemoteCodeAnalysisUnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestAttributeCheck()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxCheck()]
    public interface IFoo
    {       
    }
}
";            
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestDirectReturn()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxCheck()]
    public interface IFoo
    {       
        int Method1();
        //Regulus.Remote.Value<int> Method2();
    }
}
";
            var expected = VerifyCS.Diagnostic("RegulusRemoteCodeAnalysisReturnRule").WithSpan(7, 13, 7, 20).WithArguments("Method1");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);            
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:TypeName|}
        {   
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";

            var expected = VerifyCS.Diagnostic("RegulusRemoteCodeAnalysis").WithLocation(0).WithArguments("TypeName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
