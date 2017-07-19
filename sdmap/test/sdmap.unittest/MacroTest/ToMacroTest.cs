using FlySql.Macros;
using FlySql.unittest.MacroTest.ToMacroImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static FlySql.Macros.Implements.MacroUtil;

namespace FlySql.unittest.MacroTest
{
    public class ToMacroTest
    {
        [Fact]
        public void NameCanChange()
        {
            var macro = GetTypeMacroMethods(typeof(NameCanChangeImpl))
                .Select(ToSdmapMacro)
                .FirstOrDefault();

            Assert.Equal("NiceName", macro.Name);
        }

        [Fact]
        public void NotNullTest()
        {
            var macro = GetTypeMacroMethods(typeof(NameCanChangeImpl))
                .Select(ToSdmapMacro)
                .FirstOrDefault();

            Assert.NotNull(macro.Arguments);
            Assert.Empty(macro.Arguments);
        }

        [Fact]
        public void DetectArgumentsTest()
        {
            var macro = GetTypeMacroMethods(typeof(DetectArgumentImpl))
                .Select(ToSdmapMacro)
                .FirstOrDefault();

            Assert.Equal(new[]
            {
                SdmapTypes.Syntax, 
                SdmapTypes.Sql
            }, macro.Arguments);
        }
    }
}
