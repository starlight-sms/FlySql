﻿using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FlySql.Functional;
using FlySql.Parser.Visitor;
using FlySql.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static FlySql.Parser.G4.SdmapParser;

namespace FlySql.Compiler
{
    public class SqlEmiter
    {
        private ParserRuleContext _parseTree;
        private string _ns;
        private readonly CompileFunction _compiler;

        public EmitFunction Emiter { get; private set; }

        public SqlEmiter(
            ParserRuleContext parseTree, 
            string ns, 
            CompileFunction compiler)
        {
            _parseTree = parseTree;
            _ns = ns;
            _compiler = compiler;
        }

        public Result EnsureCompiled(SdmapCompilerContext context)
        {
            if (Emiter != null)
                return Result.Ok();

            return CompileInternal(context)
                .OnSuccess(v =>
                {
                    Emiter = v;
                    _parseTree = null;
                });
        }

        public Result<string> TryEmit(object v, SdmapCompilerContext context)
        {
            return EnsureCompiled(context)
                .OnSuccess(() => Emiter(context, v))
                .Unwrap();
        }

        public string Emit(object v, SdmapCompilerContext context)
        {
            return TryEmit(v, context).Value;
        }

        private Result<EmitFunction> CompileInternal(SdmapCompilerContext context)
        {
            if (_ns != "")
                context.NsStack.Push(_ns);

            var result = _compiler(context);

            if (_ns != "")
                context.NsStack.Pop();

            return result;
        }
    }    

    public delegate Result<EmitFunction> CompileFunction(
        SdmapCompilerContext context);

    public delegate Result<string> EmitFunction(SdmapCompilerContext context, object obj);
}
