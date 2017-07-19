﻿using Antlr4.Runtime;
using FlySql.Functional;
using FlySql.Macros;
using FlySql.Parser.G4;
using FlySql.Parser.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlySql.Compiler
{
    public class SdmapCompiler
    {
        private readonly SdmapCompilerContext _context;

        public SdmapCompiler()
        {
            _context = SdmapCompilerContext.CreateEmpty();
        }

        internal SdmapCompiler(SdmapCompilerContext context)
        {
            _context = context;
        }

        public Result AddSourceCode(string sourceCode)
        {
            var inputStream = new AntlrInputStream(sourceCode);
            var lexer = new SdmapLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new SdmapParser(tokenStream);

            var visitor = SqlItemVisitor.Create(_context);
            return visitor.Visit(parser.root());
        }

        public Result AddMacro(string id, SdmapTypes[] arguments, MacroDelegate method)
        {
            return _context.MacroManager.Add(new Macro
            {
                Name = id, 
                Arguments = arguments, 
                Method = method, 
            });
        }

        public Result AddMacro(string id, MacroDelegate method)
        {
            return _context.MacroManager.Add(new Macro
            {
                Name = id, 
                Method = method, 
                SkipArgumentRuntimeCheck = true
            });
        }

        public Result<string> TryEmit(string id, object query)
        {
            lock(_context)
            {
                return _context.TryGetEmiter(id, _context.CurrentNs)
                    .OnSuccess(emiter => emiter.TryEmit(query, _context));
            }
        }

        public string Emit(string id, object query)
        {
            return TryEmit(id, query).Value;
        }

        public Result EnsureCompiled()
        {
            foreach (var kv in _context.Emiters.ToList())
            {
                var ok = kv.Value.EnsureCompiled(_context);
                if (ok.IsFailure) return ok;
            }

            return Result.Ok();
        }
    }
}
