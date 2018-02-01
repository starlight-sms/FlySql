using System;
using System.Collections.Generic;
using System.Text;

namespace FlySql.Extensions
{
    public interface ISdmapEmiter
    {
        string Emit(string statementId, object parameters);
    }
}
