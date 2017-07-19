﻿using FlySql.Parser.Visitor;
using FlySql.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FlySql.unittest.VisitorTest
{
    public class CoreSqlVisitorBasicTest : VisitorTestBase
    {
        [Fact]
        public void HelloWorld()
        {
            var code = "sql v1{Hello World}";
            var parseTree = GetParseTree(code);
            var result = SqlEmiterUtil.CompileNamed(
                SdmapCompilerContext.CreateEmpty(),
                parseTree.namedSql()[0]);
            
            Assert.True(result.IsSuccess);

            var function = result.Value;
            var output = function(SdmapCompilerContext.CreateEmpty(), null);
            Assert.Equal("Hello World", output.Value);
        }

        [Fact]
        public void SqlInNamespaceTest()
        {
            var sql = "SELECT * FROM `client_WOReactive`;";
            var code = "sql v1{" + sql + "}";
            var parseTree = GetParseTree(code);
            var result = SqlEmiterUtil.CompileNamed(
                SdmapCompilerContext.CreateEmpty(),
                parseTree.namedSql()[0]);

            Assert.True(result.IsSuccess);

            var function = result.Value;
            var output = function(SdmapCompilerContext.CreateEmpty(), null);
            Assert.Equal(sql, output.Value);
        }

        [Fact]
        public void MultiLineTest()
        {
            var sql = 
                "SELECT                  \r\n" +
                "   *                    \r\n" +
                "FROM                    \r\n" +
                "   `client_WOReactive`; \r\n";
            var code = $"sql v1{{{sql}}}";
            var parseTree = GetParseTree(code);
            var result = SqlEmiterUtil.CompileNamed(
                SdmapCompilerContext.CreateEmpty(),
                parseTree.namedSql()[0]);

            Assert.True(result.IsSuccess);

            var function = result.Value;
            var output = function(SdmapCompilerContext.CreateEmpty(), null);
            Assert.Equal(sql, output.Value);
        }
    }
}
