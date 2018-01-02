using FlySql.Emiter.Implements.Common;
using FlySql.Functional;
using System.IO;

namespace FlySql.Emiter
{
    public interface CodeEmiterProvider
    {
        Result Emit(
            string source, 
            TextWriter writer, 
            CodeEmiterConfig config);
    }
}