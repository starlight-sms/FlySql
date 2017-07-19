﻿using Antlr4.Runtime;
using FlySql.Parser.G4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static FlySql.Parser.G4.SdmapParser;

namespace FlySql.unittest.VisitorTest
{
    public class VisitorTestBase
    {
        protected RootContext GetParseTree(string sourceCode)
        {
            return GetParser(sourceCode)
                .root();
        }

        protected SdmapParser GetParser(string sourceCode)
        {
            var inputStream = new AntlrInputStream(sourceCode);
            var lexer = new SdmapLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            return new SdmapParser(tokenStream);
        }
    }
}
