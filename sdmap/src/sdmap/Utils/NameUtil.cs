using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static FlySql.Parser.G4.SdmapParser;

namespace FlySql.Utils
{
    internal static class NameUtil
    {
        public static string GetFunctionName(ParserRuleContext context)
        {
            if (context == null) return "<Empty>";
            return "Unnamed" + HashUtil.Base64SHA256(context.GetText());
        }
    }
}
