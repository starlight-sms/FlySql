using FlySql.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FlySql.IntegratedTest
{
    public class CompareTest
    {
        [Fact]
        public void IsEqualOk()
        {
            var code = "sql v1{#isEqual<A, true, 'Yes'>}";
            var rt = new SdmapCompiler();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", new { A = true });
            Assert.Equal("Yes", result);
        }
    }
}
