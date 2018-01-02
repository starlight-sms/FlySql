using FlySql.Emiter.Implements.Common;
using FlySql.Functional;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlySql.Emiter
{
    public class CodeEmiter
    {
        public Result Emit(
            string source, TextWriter writer, CodeEmiterConfig config, CodeEmiterProvider codeEmiterProvider)
        {
            return codeEmiterProvider.Emit(source, writer, config);
        }
    }
}
