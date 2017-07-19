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
    public class HasPropTest
    {
        [Fact]
        public void HasPropShouldEmit()
        {
            var val = CallHasProp(new { A = 3 }, "A", "test");
            Assert.True(val.IsSuccess);
            Assert.Equal("test", val.Value);
        }

        [Fact]
        public void NoPropShouldNotEmit()
        {
            var val = CallHasProp(new { A = 3 }, "B", "test");
            Assert.True(val.IsSuccess);
            Assert.Equal("", val.Value);
        }

        [Fact]
        public void NullShouldFail()
        {
            var val = CallHasProp(null, "B", "test");
            Assert.False(val.IsSuccess);
        }

        private Result<string> CallHasProp(object self, string prop, string result)
        {
            return CommonMacros.HasProp(SdmapCompilerContext.CreateEmpty(), "", self, new[] { prop, result });
        }
    }
}
