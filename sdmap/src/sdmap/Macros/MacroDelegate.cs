using FlySql.Functional;
using FlySql.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlySql.Macros
{
    public delegate Result<string> MacroDelegate(SdmapCompilerContext context, 
        string ns, object self, object[] arguments);
}
