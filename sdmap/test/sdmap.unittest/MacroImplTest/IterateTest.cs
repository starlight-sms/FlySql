﻿using FlySql.Functional;
using FlySql.Macros.Implements;
using FlySql.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FlySql.unittest.MacroImplTest
{
    public class IterateTest
    {
        [Fact]
        public void IterateString()
        {
            var actual = CallIterate(Enumerable.Range(1, 3), ",", "A");
            Assert.True(actual.IsSuccess);
            Assert.Equal("A,A,A", actual.Value);
        }

        [Fact]
        public void IterateFunction()
        {
            EmitFunction ef = (SdmapCompilerContext ctx, object o) => 
            {
                return Result.Ok(o.ToString());
            };
            var actual = CallIterate(Enumerable.Range(1, 3), ",", ef);
            Assert.True(actual.IsSuccess);
            Assert.Equal("1,2,3", actual.Value);
        }

        [Fact]
        public void EachString()
        {
            var obj = new { A = Enumerable.Range(1, 3) };
            var actual = CallEach(obj, "A", ",", "A");
            Assert.True(actual.IsSuccess);
            Assert.Equal("A,A,A", actual.Value);
        }

        [Fact]
        public void EachFunction()
        {
            EmitFunction ef = (SdmapCompilerContext ctx, object o) =>
            {
                return Result.Ok(o.ToString());
            };
            var obj = new { A = Enumerable.Range(1, 3) };
            var actual = CallEach(obj, "A", ",", ef);
            Assert.True(actual.IsSuccess);
            Assert.Equal("1,2,3", actual.Value);
        }

        private Result<string> CallIterate(object self, string joiner, EmitFunction ef)
        {
            return DynamicRuntimeMacros.Iterate(SdmapCompilerContext.CreateEmpty(), 
                "", self, new object[] { joiner, ef });
        }

        private Result<string> CallIterate(object self, string joiner, string text)
        {
            return DynamicRuntimeMacros.Iterate(SdmapCompilerContext.CreateEmpty(),
                "", self, new object[] { joiner, text });
        }

        private Result<string> CallEach(object self, 
            string prop, string joiner, EmitFunction ef)
        {
            return DynamicRuntimeMacros.Each(SdmapCompilerContext.CreateEmpty(),
                "", self, new object[] { prop, joiner, ef });
        }

        private Result<string> CallEach(object self, 
            string prop, string joiner, string text)
        {
            return DynamicRuntimeMacros.Each(SdmapCompilerContext.CreateEmpty(),
                "", self, new object[] { prop, joiner, text });
        }
    }
}
