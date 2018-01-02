using FlySql.Functional;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlySql.Emiter.Implements.CSharp
{
    public interface ISdmapEmiter
    {
        Result<string> BuildText(dynamic self);
    }
}
