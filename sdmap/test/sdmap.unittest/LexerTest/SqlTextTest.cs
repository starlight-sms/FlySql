using Antlr4.Runtime;
using FlySql.Parser.G4;
using FlySql.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

using static FlySql.Parser.G4.SdmapLexer;

namespace sdmap.unittest.LexerTest
{
    public class SqlTextTest
    {
        [Fact]
        public void DoubleHashIsHash()
        {
            var code = "sql v1{##}";
            var ats = new AntlrInputStream(code);
            var lexer = new SdmapLexer(ats);
            var tokens = lexer.GetAllTokens();
            Assert.Equal(
                new[] { KSql, SYNTAX, OpenCurlyBrace, SQLText, CloseSql },
                tokens.Select(x => x.Type));
        }

        [Fact]
        public void SingleHashIsMacro()
        {
            var code = "sql v1{#}";
            var ats = new AntlrInputStream(code);
            var lexer = new SdmapLexer(ats);
            var tokens = lexer.GetAllTokens();
            Assert.Equal(
                new[] { KSql, SYNTAX, OpenCurlyBrace, Hash, CloseCurlyBrace },
                tokens.Select(x => x.Type));
        }

        [Fact]
        public void DoubleHashWillEmitSingleHash()
        {
            var sqlText = "##";
            var result = SqlTextUtil.Parse(sqlText);
            Assert.Equal("#", result);
        }
    }
}
