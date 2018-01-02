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
            return DynamicRuntimeMacros.HasProp(SdmapCompilerContext.CreateEmpty(), "", self, new[] { prop, result });
        }        
    }

    public class HasNoPropTest
    {
        [Fact]
        public void HasNoPropShouldEmit()
        {
            var val = CallHasNoProp(new { A = 3 }, "B", "test");
            Assert.True(val.IsSuccess);
            Assert.Equal("test", val.Value);
        }

        [Fact]
        public void HasPropShouldNotEmit()
        {
            var val = CallHasNoProp(new { A = 3 }, "A", "test");
            Assert.True(val.IsSuccess);
            Assert.Equal("", val.Value);
        }

        [Fact]
        public void NullShouldFail()
        {
            var val = CallHasNoProp(null, "A", "test");
            Assert.False(val.IsSuccess);
        }

        private Result<string> CallHasNoProp(object self, string prop, string result)
        {
            return DynamicRuntimeMacros.HasNoProp(SdmapCompilerContext.CreateEmpty(), "", self, new[] { prop, result });
        }
    }
}
