using System;
using System.Collections.Generic;
using System.Text;

namespace FlySql.ext
{
    public interface ISqlEmiter
    {
        string EmitSql(string sqlMapName, object queryObject);
    }
}
