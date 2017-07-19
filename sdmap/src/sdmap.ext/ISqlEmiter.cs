using System;
using System.Collections.Generic;
using System.Text;

namespace FlySql.Extensions
{
    public interface ISqlEmiter
    {
        string EmitSql(string sqlMapName, object queryObject);
    }
}
