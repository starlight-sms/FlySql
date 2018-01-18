﻿using sdmap.Functional;
using sdmap.Macros.Implements;
using sdmap.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace sdmap.unittest.MacroImplTest
{
    public class IsEqualTest
    {
        [Fact]
        public void Smoke()
        {
            var val = CallIsEqual(new { A = "Val" }, "A", "Val", "Ok");
            Assert.True(val.IsSuccess);
            Assert.Equal("Ok", val.Value);
        }

        [Fact]
        public void IntEqualInt()
        {
            var val = CallIsEqual(new { A = 3 }, "A", 3, "Ok");
            Assert.True(val.IsSuccess);
            Assert.Equal("Ok", val.Value);
        }

        [Fact]
        public void DoubleEqualDouble()
        {
            var val = CallIsEqual(new { A = 3.0 }, "A", 3.0, "Ok");
            Assert.True(val.IsSuccess);
            Assert.Equal("Ok", val.Value);
        }

        private Result<string> CallIsEqual(object self, string prop, object val, string result)
        {
            if (val is int) val = Convert.ToDouble(val);
            return DynamicRuntimeMacros.IsEqual(SdmapCompilerContext.CreateEmpty(), 
                "", self, new object[] { prop, val, result });
        }

        private Result<string> CallIsNotEqual(object self, string prop, object val, string result)
        {
            if (val is int) val = Convert.ToDouble(val);
            return DynamicRuntimeMacros.IsNotEqual(SdmapCompilerContext.CreateEmpty(), 
                "", self, new object[] { prop, val, result });
        }
    }
}
