using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlySql.Macros.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MacroArgumentsAttribute : Attribute
    {
        public SdmapTypes[] Arguments { get; }

        public MacroArgumentsAttribute(params SdmapTypes[] arguments)
        {
            Arguments = arguments;
        }
    }
}
