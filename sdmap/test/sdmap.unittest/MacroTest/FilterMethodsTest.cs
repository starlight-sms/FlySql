﻿using FlySql.Macros;
using FlySql.Macros.Implements;
using FlySql.unittest.MacroTest.FilterMethodsImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FlySql.unittest.MacroTest
{
    public class FilterMethodsTest
    {
        [Fact]
        public void HelloWorld()
        {
            var method = MacroUtil.GetTypeMacroMethods(typeof(HelloWorldImpl)).FirstOrDefault();
            Assert.NotNull(method);
            Assert.Equal(nameof(HelloWorldImpl.HelloWorld), method.Name);
        }

        [Fact]
        public void ReturnCheck()
        {
            var methods = MacroUtil.GetTypeMacroMethods(typeof(ReturnCheckImpl)).ToList();
            Assert.Equal(1, methods.Count);
            Assert.Equal(nameof(ReturnCheckImpl.Ok), methods[0].Name);
        }

        [Fact]
        public void ParameterCheck()
        {
            var methods = MacroUtil.GetTypeMacroMethods(typeof(ParameterCheckImpl)).ToList();
            Assert.Equal(1, methods.Count);
            Assert.Equal(nameof(ParameterCheckImpl.Ok), methods[0].Name);
        }
    }
}
