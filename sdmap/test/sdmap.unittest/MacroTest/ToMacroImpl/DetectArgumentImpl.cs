﻿using FlySql.Functional;
using FlySql.Macros;
using FlySql.Macros.Attributes;
using FlySql.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlySql.unittest.MacroTest.ToMacroImpl
{
    public static class DetectArgumentImpl
    {
        [MacroArguments(SdmapTypes.Syntax, SdmapTypes.Sql)]
        public static Result<string> DetectMe(SdmapCompilerContext context, string ns, object self, object[] arguments)
        {
            return Result.Ok("Hello World");
        }
    }
}
