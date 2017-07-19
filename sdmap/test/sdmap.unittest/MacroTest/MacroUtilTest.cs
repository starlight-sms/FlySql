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
            var prop = CommonMacros.GetProp(val, "A");
            Assert.NotNull(prop);
        }

        [Fact]
        public void GetNestedPropOk()
        {
            var val = new { A = new { A = 3 } };
            var prop = CommonMacros.GetProp(val, "A.A");
            Assert.NotNull(prop);
        }

        [Fact]
        public void GetNotExistPropWillReturnNull()
        {
            var val = new { A = 3 };
            var prop = CommonMacros.GetProp(val, "A.A");
            Assert.Null(prop);
        }

        [Fact]
        public void GetNotExistPropWillNotThrow()
        {
            var val = new { A = 3 };
            var prop = CommonMacros.GetProp(val, "B.C.D");
        }

        [Fact]
        public void CanGetNextedObjectValue()
        {
            var val = new { A = new { B = 4 } };
            var getted = CommonMacros.GetPropValue(val, "A.B");
            Assert.Equal(4, getted);
        }

        [Fact]
        public void CanDetectEmptyArray()
        {
            var arr = Enumerable.Range(1, 0);
            Assert.True(CommonMacros.ArrayEmpty(arr));
        }

        [Fact]
        public void CanDetectNotEmptyArray()
        {
            var arr = Enumerable.Range(1, 10);
            Assert.False(CommonMacros.ArrayEmpty(arr));
        }

        [Fact]
        public void BoolEqualBool()
        {
            Assert.True(CommonMacros.IsEqual(true, true));
        }

        [Fact]
        public void NumberEqualsNumber()
        {
            Assert.True(CommonMacros.IsEqual(3.14m, 3.14));
        }

        [Fact]
        public void DateEqualsDate()
        {
            var date = new DateTime(2016, 1, 1);
            Assert.True(CommonMacros.IsEqual(date, DateUtil.Parse("2016/1/1").Value));
        }
    }
}
