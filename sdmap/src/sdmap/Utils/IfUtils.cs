﻿using FlySql.Compiler;
using FlySql.Functional;
using FlySql.Macros.Implements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FlySql.Utils
{
    internal static class IfUtils
    {
        public static bool PropertyExistsAndEvalToTrue(object obj, string propName)
        {
            var val = DynamicRuntimeMacros.GetPropValue(obj, propName);
            if (val is bool) return (bool)val;
            if (val is bool?) return ((bool?)val).GetValueOrDefault();

            return false;
        }

        public static object LoadProp(object obj, string propName)
            => DynamicRuntimeMacros.GetPropValue(obj, propName);

        public static Result<string> ExecuteEmiter(EmitFunction ef, SdmapCompilerContext ctx, object obj)
            => ef(ctx, obj);

        public static bool IsEmpty(object obj)
            => RuntimeMacros.IsEmpty(obj);
    }
}
