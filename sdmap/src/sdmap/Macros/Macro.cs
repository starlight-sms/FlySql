﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FlySql.Macros
{
    public class Macro
    {
        public string Name { get; set; }

        public bool SkipArgumentRuntimeCheck { get; set; }

        public SdmapTypes[] Arguments { get; set; }

        public MacroDelegate Method { get; set; }
    }
}
