﻿using FlySql.Functional;
using FlySql.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlySql.unittest.MacroTest.FilterMethodsImpl
{
    public class ReturnCheckImpl
    {
        public static string NotCorrect(SdmapCompilerContext context, string ns, object self, object[] arguments)
        {
            return "Hello World";
        }

        public static Result<string> Ok(SdmapCompilerContext context, string ns, object self, object[] arguments)
        {
            return Result.Ok("Hello World");
        }
    }
}
