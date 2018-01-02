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

namespace FlySql.Parser.Visitor
{
    internal class CoreSqlVisitor : SdmapParserBaseVisitor<Result>
    {
        protected readonly SdmapCompilerContext _context;
        protected ILGenerator _il;
        protected int _stackPos;

        public EmitFunction Function { get; protected set; }

        public CoreSqlVisitor(
            SdmapCompilerContext context)
        {
            _context = context;
        }

        public Result Process(CoreSqlContext parseRule, string functionName)
        {
            var method = new DynamicMethod(functionName,
                typeof(Result<string>), new[] { typeof(SdmapCompilerContext), typeof(object) });
            _il = method.GetILGenerator();

            void returnBlock()
            {
                _il.Emit(OpCodes.Ldloc_0);                                        // sb
                var okMethod = typeof(Result)
                    .GetTypeInfo()
                    .GetMethods()
                    .Single(x => x.IsGenericMethod && x.Name == "Ok")
                    .MakeGenericMethod(typeof(string));
                _il.Emit(OpCodes.Call, typeof(StringBuilder)
                    .GetTypeInfo()
                    .GetMethod(nameof(StringBuilder.ToString), Type.EmptyTypes)); // str
                _il.Emit(OpCodes.Call, okMethod);                                 // result<str>                

                _il.Emit(OpCodes.Ret);                                            // [empty-returned]
                Function = (EmitFunction)method.CreateDelegate(typeof(EmitFunction));
            };

            if (parseRule == null)
            {
                returnBlock();
                return Result.Ok();
            }

            _il.DeclareLocal(typeof(StringBuilder));
            _il.Emit(OpCodes.Newobj, typeof(StringBuilder)
                .GetTypeInfo()
                .GetConstructor(Type.EmptyTypes));                                    // sb
            _il.Emit(OpCodes.Stloc_0);                                                // [empty]

            return Visit(parseRule)
                .OnSuccess(() =>                                                      // [must be empty]
                {
                    returnBlock();
                });
        }

        public override Result VisitMacro([NotNull] MacroContext context)
        {
            var macroName = context.GetToken(SYNTAX, 0).GetText();

            _il.Emit(OpCodes.Ldarg_0);                                      // ctx
            _il.Emit(OpCodes.Ldstr, macroName);                             // ctx name
            _il.Emit(OpCodes.Ldstr, _context.CurrentNs);                    // ctx name ns
            _il.Emit(OpCodes.Ldarg_1);                                      // ctx name ns self

            var contexts = context.GetRuleContexts<MacroParameterContext>();
            _il.Emit(OpCodes.Ldc_I4, contexts.Length);                      // ctx name ns self
            _il.Emit(OpCodes.Newarr, typeof(object));                       // ctx name ns self args
            for (var i = 0; i < contexts.Length; ++i)
            {
                var arg = contexts[i];

                _il.Emit(OpCodes.Dup);                                      // .. -> args
                _il.Emit(OpCodes.Ldc_I4, i);                                // .. -> args idx

                if (arg.nsSyntax() != null)
                {
                    _il.Emit(OpCodes.Ldstr, arg.nsSyntax().GetText());      // .. -> args idx ele
                }
                else if (arg.STRING() != null)
                {
                    var result = StringUtil.Parse(arg.STRING().GetText());  // .. -> args idx ele
                    if (result.IsSuccess)
                    {
                        _il.Emit(OpCodes.Ldstr, result.Value);              // .. -> args idx ele
                    }
                    else
                    {
                        return result;
                    }
                }
                else if (arg.NUMBER() != null)
                {
                    var result = NumberUtil.Parse(arg.NUMBER().GetText());
                    if (result.IsSuccess)
                    {
                        _il.Emit(OpCodes.Ldc_R8, result.Value);             // .. -> args idx vele
                        _il.Emit(OpCodes.Box, typeof(double));              // .. -> args idx rele
                    }
                    else
                    {
                        return result;
                    }
                }
                else if (arg.DATE() != null)
                {
                    var result = DateUtil.Parse(arg.DATE().GetText());
                    if (result.IsSuccess)
                    {
                        _il.Emit(OpCodes.Ldc_I8, result.Value.ToBinary());  // .. -> args idx int64
                        var ctor = typeof(DateTime).GetTypeInfo().GetConstructor(new[] { typeof(long) });
                        _il.Emit(OpCodes.Newobj, ctor);                     // .. -> args idx date
                        _il.Emit(OpCodes.Box, typeof(DateTime));            // .. -> args idx rele
                    }
                    else
                    {
                        return result;
                    }
                }
                else if (arg.Bool() != null)
                {
                    _il.Emit(bool.Parse(arg.Bool().GetText()) ?
                        OpCodes.Ldc_I4_1 :
                        OpCodes.Ldc_I4_0);                                  // .. -> args idx bool
                    _il.Emit(OpCodes.Box, typeof(bool));                    // .. -> args idx rele
                }
                else if (arg.unnamedSql() != null)
                {
                    var parseTree = arg.unnamedSql();
                    var id = NameUtil.GetFunctionName(parseTree);
                    var result = _context.TryGetEmiter(id, _context.CurrentNs);

                    SqlEmiter emiter;
                    if (result.IsSuccess)
                    {
                        emiter = result.Value;
                    }
                    else
                    {
                        emiter = SqlEmiterUtil.CreateUnnamed(parseTree, _context.CurrentNs);
                        var ok = _context.TryAdd(id, emiter);
                        if (ok.IsFailure) return ok;
                    }

                    var compileResult = emiter.EnsureCompiled(_context);
                    if (compileResult.IsFailure)
                    {
                        return compileResult;
                    }

                    _il.Emit(OpCodes.Ldarg_0);                             // .. -> args idx ctx
                    _il.Emit(OpCodes.Ldstr, id);                           // .. -> args idx ctx id
                    _il.Emit(OpCodes.Ldstr, _context.CurrentNs);           // .. -> args idx ctx id ns
                    _il.Emit(OpCodes.Call, typeof(SqlEmiterUtil).GetTypeInfo()
                        .GetMethod(nameof(SqlEmiterUtil.EmiterFromId)));// .. -> args idx emiter
                }
                else
                {
                    throw new InvalidOperationException();
                }

                _il.Emit(OpCodes.Stelem_Ref);                               // -> ctx name ns self args
            }

            _il.Emit(OpCodes.Call, typeof(MacroManager).GetTypeInfo()
                .GetMethod(nameof(MacroManager.Execute)));                  // result<str>
            _il.Emit(OpCodes.Dup);                                          // result<str> x 2
            _il.Emit(OpCodes.Call, typeof(Result).GetTypeInfo()
                .GetMethod("get_" + nameof(Result.IsSuccess)));             // result<str> bool
            _il.Emit(OpCodes.Ldc_I4_1);                                     // result<str> bool true
            var ifIsSuccess = _il.DefineLabel();
            _il.Emit(OpCodes.Beq, ifIsSuccess);                             // result<str> (jmp if equal)
            _il.Emit(OpCodes.Ret);                                          // [exit-returned]

            _il.MarkLabel(ifIsSuccess);                                     // ifIsSuccess:
            _il.Emit(OpCodes.Call, typeof(Result<string>).GetTypeInfo()
                .GetMethod("get_" + nameof(Result<string>.Value)));         // str
            var strValue = _il.DeclareLocal(typeof(string));
            _il.Emit(OpCodes.Stloc, strValue);                              // [empty]
            _il.Emit(OpCodes.Ldloc_0);                                      // sb
            _il.Emit(OpCodes.Ldloc, strValue);                              // sb str
            _il.Emit(OpCodes.Call, typeof(StringBuilder)
                .GetTypeInfo().GetMethod(nameof(StringBuilder.Append),
                new[] { typeof(string), }));                                // sb+str
            _il.Emit(OpCodes.Pop);                                          // [empty]

            return Result.Ok();
        }

        public override Result VisitPlainText([NotNull] PlainTextContext context)
        {
            var text = SqlTextUtil.Parse(context.GetToken(SQLText, 0).GetText());

            _il.Emit(OpCodes.Ldloc_0);                                             // sb
            _il.Emit(OpCodes.Ldstr, text);                                         // sb str
            _il.Emit(OpCodes.Call, typeof(StringBuilder)
                .GetTypeInfo().GetMethod(nameof(StringBuilder.Append),
                new[] { typeof(string), }));                                       // sb+str
            _il.Emit(OpCodes.Pop);                                                 // [empty]
            return Result.Ok();
        }

        public override Result VisitIf([NotNull] IfContext context)
        {
            var coreSql = context.coreSql();
            var id = NameUtil.GetFunctionName(coreSql);
            var result = _context.TryGetEmiter(id, _context.CurrentNs);

            SqlEmiter emiter;
            if (result.IsSuccess)
            {
                emiter = result.Value;
            }
            else
            {
                emiter = SqlEmiterUtil.CreateCore(coreSql, _context.CurrentNs);
                var ok = _context.TryAdd(id, emiter);
                if (ok.IsFailure) return ok;
            }

            var compileResult = emiter.EnsureCompiled(_context);
            if (compileResult.IsFailure)
            {
                return compileResult;
            }

            return new BoolVisitor(_il).Visit(context.boolExpression())
                .OnSuccess(() =>
                {
                    var ifSkip = _il.DefineLabel();
                    _il.Emit(OpCodes.Ldc_I4_0);
                    _il.Emit(OpCodes.Beq, ifSkip);                    

                    _il.Emit(OpCodes.Ldarg_0);                             // ctx
                    _il.Emit(OpCodes.Ldstr, id);                           // ctx id
                    _il.Emit(OpCodes.Ldstr, _context.CurrentNs);           // ctx id ns
                    _il.Emit(OpCodes.Call, typeof(SqlEmiterUtil).GetTypeInfo()
                        .GetMethod(nameof(SqlEmiterUtil.EmiterFromId)));// emiter
                    _il.Emit(OpCodes.Ldarg_0);                             // emiter ctx
                    _il.Emit(OpCodes.Ldarg_1);                             // emiter ctx obj
                    _il.Emit(OpCodes.Call,                                 // result<str>
                        typeof(IfUtils).GetTypeInfo().GetMethod(nameof(IfUtils.ExecuteEmiter)));

                    // convert result<str> to str
                    _il.Emit(OpCodes.Dup);                                     // result<str> x 2
                    _il.Emit(OpCodes.Call, typeof(Result).GetTypeInfo()
                        .GetMethod("get_" + nameof(Result.IsSuccess)));        // result<str> bool
                    _il.Emit(OpCodes.Ldc_I4_1);                                // result<str> bool true
                    var ifIsSuccess = _il.DefineLabel();
                    _il.Emit(OpCodes.Beq, ifIsSuccess);                        // result<str> (jmp if equal)
                    _il.Emit(OpCodes.Ret);                                     // [exit-returned]

                    _il.MarkLabel(ifIsSuccess);                                // ifIsSuccess:
                    _il.Emit(OpCodes.Call, typeof(Result<string>).GetTypeInfo()
                        .GetMethod("get_" + nameof(Result<string>.Value)));    // str

                    var strLocal = _il.DeclareLocal(typeof(string));
                    _il.Emit(OpCodes.Stloc, strLocal);
                    _il.Emit(OpCodes.Ldloc_0);
                    _il.Emit(OpCodes.Ldloc, strLocal);
                    _il.Emit(OpCodes.Call, 
                        typeof(StringBuilder).GetTypeInfo().GetMethod(nameof(StringBuilder.Append),
                        new[] { typeof(string), }));                                // sb+str
                    _il.Emit(OpCodes.Pop);
                    _il.MarkLabel(ifSkip);
                    return Result.Ok();
                });
        }

        protected override Result AggregateResult(Result aggregate, Result nextResult)
        {
            return Result.Combine(new[]
            {
                aggregate, 
                nextResult
            });
        }
        

        public static Result<EmitFunction> CompileCore(
            CoreSqlContext coreSql, 
            SdmapCompilerContext context, 
            string functionName)
        {
            var visitor = new CoreSqlVisitor(context);
            return visitor.Process(coreSql, functionName)
                .OnSuccess(() => visitor.Function);
        }
    }
}
