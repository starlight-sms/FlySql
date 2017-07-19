using FlySql.Functional;
using FlySql.Macros.Attributes;
using FlySql.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlySql.unittest.MacroTest.ToMacroImpl
{
    public static class NameCanChangeImpl
    {
        [Macro("NiceName")]
        public static Result<string> HelloWorld(SdmapCompilerContext context, string ns, object self, object[] arguments)
        {
            return Result.Ok("Hello World");
        }
    }
}
