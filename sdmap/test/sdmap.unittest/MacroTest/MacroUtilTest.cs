﻿using FlySql.Macros.Implements;
using FlySql.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FlySql.unittest.MacroTest
{
    public class MacroUtilTest
    {
        [Fact]
        public void GetSimplePropOk()
        {
            var val = new { A = 3 };
            var prop = DynamicRuntimeMacros.GetProp(val, "A");
            Assert.NotNull(prop);
        }

        [Fact]
        public void GetNestedPropOk()
        {
            var val = new { A = new { A = 3 } };
            var prop = DynamicRuntimeMacros.GetProp(val, "A.A");
            Assert.NotNull(prop);
        }

        [Fact]
        public void GetNotExistPropWillReturnNull()
        {
            var val = new { A = 3 };
            var prop = DynamicRuntimeMacros.GetProp(val, "A.A");
            Assert.Null(prop);
        }

        [Fact]
        public void GetNotExistPropWillNotThrow()
        {
            var val = new { A = 3 };
            var prop = DynamicRuntimeMacros.GetProp(val, "B.C.D");
        }

        [Fact]
        public void CanGetNextedObjectValue()
        {
            var val = new { A = new { B = 4 } };
            var getted = DynamicRuntimeMacros.GetPropValue(val, "A.B");
            Assert.Equal(4, getted);
        }

        [Fact]
        public void CanDetectEmptyArray()
        {
            var arr = Enumerable.Range(1, 0);
            Assert.True(RuntimeMacros.ArrayEmpty(arr));
        }

        [Fact]
        public void CanDetectNotEmptyArray()
        {
            var arr = Enumerable.Range(1, 10);
            Assert.False(RuntimeMacros.ArrayEmpty(arr));
        }

        [Fact]
        public void BoolEqualBool()
        {
            Assert.True(DynamicRuntimeMacros.IsEqual(true, true));
        }

        [Fact]
        public void NumberEqualsNumber()
        {
            Assert.False(DynamicRuntimeMacros.IsEqual(3.14m, 3.14));
        }

        [Fact]
        public void DecimalEqualToDecimal()
        {
            Assert.True(DynamicRuntimeMacros.IsEqual(3.14m, 3.14m));
        }

        [Fact]
        public void DateEqualsDate()
        {
            var date = new DateTime(2016, 1, 1);
            Assert.True(DynamicRuntimeMacros.IsEqual(date, DateUtil.Parse("2016/1/1").Value));
        }
    }
}
