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
            var val = CommonMacros.GetPropValue(obj, propName);
            if (val is bool) return (bool)val;
            if (val is bool?) return ((bool?)val).GetValueOrDefault();

            return false;
        }

        public static object LoadProp(object obj, string propName)
        {
            return CommonMacros.GetPropValue(obj, propName);
        }

        public static Result<string> ExecuteEmiter(EmitFunction ef, SdmapCompilerContext ctx, object obj)
        {
            return ef(ctx, obj);
        }

        public static bool IsEmpty(object obj)
        {
            if (obj == null) return true;
            if (obj is string) return string.IsNullOrEmpty((string)obj);
            if (obj is IEnumerable) return CommonMacros.ArrayEmpty(obj);
            return false;
        }
    }
}
