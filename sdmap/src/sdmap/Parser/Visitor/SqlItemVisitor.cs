﻿using Antlr4.Runtime.Misc;
using FlySql.Functional;
using FlySql.Parser.G4;
using FlySql.Utils;
using FlySql.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static FlySql.Parser.G4.SdmapLexer;

namespace FlySql.Parser.Visitor
{
    internal class SqlItemVisitor : SdmapParserBaseVisitor<Result>
    {
        public SdmapCompilerContext Context { get; }

        private SqlItemVisitor(SdmapCompilerContext context)
        {
            Context = context;
        }

        public override Result VisitNamespace([NotNull] SdmapParser.NamespaceContext context)
        {
            var ns = context.nsSyntax().GetText();

            Context.NsStack.Push(ns);
            var result = base.VisitNamespace(context);
            Context.NsStack.Pop();
            return result;
        }

        public override Result VisitNamedSql([NotNull] SdmapParser.NamedSqlContext context)
        {
            var id = context.GetToken(SYNTAX, 0).GetText();

            return Context.TryAdd(id, SqlEmiterUtil.CreateNamed(context, Context.CurrentNs));
        }

        public static SqlItemVisitor Create(SdmapCompilerContext context)
        {
            return new SqlItemVisitor(context);
        }

        public static SqlItemVisitor CreateEmpty()
        {
            return new SqlItemVisitor(SdmapCompilerContext.CreateEmpty());
        }
    }
}
