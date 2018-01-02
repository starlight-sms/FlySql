using System;
using System.Collections.Generic;
using System.Text;

namespace FlySql.Emiter.Implements.Common
{
    public class CodeEmiterConfig
    {
        public string Namespace { get; set; } = "";

        public string AccessModifier { get; set; } = "internal";
    }
}
