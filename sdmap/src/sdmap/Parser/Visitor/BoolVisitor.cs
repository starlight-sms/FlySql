﻿using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FlySql.Functional;
using FlySql.Macros;
using FlySql.Parser.G4;
using FlySql.Utils;
using FlySql.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static FlySql.Parser.G4.SdmapParser;
using FlySql.Macros.Implements;

namespace FlySql.Parser.Visitor
{
    internal class BoolVisitor : SdmapParserBaseVisitor<Result>
    {
        private readonly ILGenerator _il;

        public BoolVisitor(ILGenerator il)
        {
            _il = il;
        }

        public override Result VisitBoolNull([NotNull] BoolNullContext context)
        {
            var op = context.children[1].GetText();
            _il.Emit(OpCodes.Ldarg_1);                              // self
            _il.Emit(OpCodes.Ldstr, context.children[0].GetText()); // self propName
            _il.Emit(OpCodes.Call, typeof(IfUtils).GetTypeInfo().GetMethod(
                nameof(IfUtils.LoadProp)));                         // obj
            switch (op)
            {
                case "==":
                    _il.Emit(OpCodes.Ldnull);
                    _il.Emit(OpCodes.Ceq);
                    return Result.Ok();
                case "!=":
                    _il.Emit(OpCodes.Ldnull);
                    _il.Emit(OpCodes.Ceq);
                    _il.Emit(OpCodes.Ldc_I4_0);
                    _il.Emit(OpCodes.Ceq);
                    return Result.Ok();
            }
            return Result.Fail($"Operator '{op}' is not allowed in bool-null expression.");
        }

        public override Result VisitBoolNsSyntax([NotNull] BoolNsSyntaxContext context)
        {
            _il.Emit(OpCodes.Ldarg_1);                              // stack: self
            _il.Emit(OpCodes.Ldstr, context.children[0].GetText()); // stack: self propName
            _il.Emit(OpCodes.Call, typeof(IfUtils).GetTypeInfo().GetMethod(
                nameof(IfUtils.PropertyExistsAndEvalToTrue)));
            return Result.Ok();
        }

        public override Result VisitBoolLeteral([NotNull] BoolLeteralContext context)
        {
            if (bool.TryParse(context.GetText(), out bool b))
            {
                if (b)
                    _il.Emit(OpCodes.Ldc_I4_1);
                else
                    _il.Emit(OpCodes.Ldc_I4_0);
                return Result.Ok();
            }
            
            return Result.Fail($"Failed to parse '{context.GetText()}' as bool");
        }

        public override Result VisitBoolOpAnd([NotNull] BoolOpAndContext context)
        {
            var end = _il.DefineLabel();
            var gofalse = _il.DefineLabel();

            var exp1 = Visit(context.boolExpression()[0]);
            if (exp1.IsFailure) return exp1;
            _il.Emit(OpCodes.Brfalse_S, gofalse);

            var exp2 = Visit(context.boolExpression()[1]);
            if (exp2.IsFailure) return exp2;
            _il.Emit(OpCodes.Brfalse_S, gofalse);

            _il.Emit(OpCodes.Ldc_I4_1);
            _il.Emit(OpCodes.Br_S, end);

            _il.MarkLabel(gofalse);
            _il.Emit(OpCodes.Ldc_I4_0);

            _il.MarkLabel(end);
            return Result.Ok();
        }

        public override Result VisitBoolOpOr([NotNull] BoolOpOrContext context)
        {
            var end = _il.DefineLabel();
            var gotrue = _il.DefineLabel();

            var exp1 = Visit(context.boolExpression()[0]);
            if (exp1.IsFailure) return exp1;
            _il.Emit(OpCodes.Brtrue_S, gotrue);

            var exp2 = Visit(context.boolExpression()[1]);
            if (exp2.IsFailure) return exp2;
            _il.Emit(OpCodes.Brtrue_S, gotrue);

            _il.Emit(OpCodes.Ldc_I4_0);
            _il.Emit(OpCodes.Br_S, end);

            _il.MarkLabel(gotrue);
            _il.Emit(OpCodes.Ldc_I4_1);

            _il.MarkLabel(end);
            return Result.Ok();
        }

        public override Result VisitBoolOpNot([NotNull] BoolOpNotContext context)
        {
            var exp = Visit(context.boolExpression());
            if (exp.IsFailure) return exp;
            _il.Emit(OpCodes.Ldc_I4_0);
            _il.Emit(OpCodes.Ceq);
            return Result.Ok();
        }

        public override Result VisitBoolFunc([NotNull] BoolFuncContext context)
        {
            var syntax = context.GetToken(SYNTAX, 0).GetText();
            var exps = context.nsSyntax();

            foreach (var exp in exps)
            {
                _il.Emit(OpCodes.Ldarg_1);
                _il.Emit(OpCodes.Ldstr, exp.GetText());
                _il.Emit(OpCodes.Call, typeof(DynamicRuntimeMacros).GetTypeInfo()
                    .GetMethod(nameof(DynamicRuntimeMacros.GetPropValue)));
            }

            switch (syntax)
            {
                case "isEmpty":
                    if (exps.Length != 1) break;
                    _il.Emit(OpCodes.Call, typeof(IfUtils).GetTypeInfo()
                        .GetMethod(nameof(IfUtils.IsEmpty)));
                    return Result.Ok();
                case "isNotEmpty":
                    if (exps.Length != 1) break;
                    _il.Emit(OpCodes.Call, typeof(IfUtils).GetTypeInfo()
                        .GetMethod(nameof(IfUtils.IsEmpty)));
                    _il.Emit(OpCodes.Ldc_I4_0);
                    _il.Emit(OpCodes.Ceq);
                    return Result.Ok();
            }
            return Result.Fail(
                $"Function '{syntax}' with {exps.Length} arguments is not supported in bool expression.");
        }

        public override Result VisitBoolBrace([NotNull] BoolBraceContext context)
        {
            return Visit(context.boolExpression());
        }
    }
}
