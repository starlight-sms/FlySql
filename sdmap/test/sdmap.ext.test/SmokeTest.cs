using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FlySql.Extensions.test
{
    public class SmokeTest
    {
        private class SimpleSqlEmiter : ISqlEmiter
        {
            public string EmitSql(string sqlId, object queryObject)
            {
                return sqlId;
            }
        }

        [Fact]
        public void SqlEmiterTest()
        {
            FlySqlExtensions.SetSqlEmiter(new SimpleSqlEmiter());
            var actual = FlySqlExtensions.EmitSql("test", null);
            Assert.Equal("test", actual);
        }

        [Fact]
        public void WatchSmoke()
        {
            Directory.CreateDirectory("sqls");
            var tempFile = @"sqls\test.sdmap";
            File.WriteAllText(tempFile, "sql Hello{Hello}");
            FlySqlExtensions.SetSqlDirectoryAndWatch(@".\sqls");

            try
            {
                File.WriteAllText(tempFile, "sql Hello{Hello2}");
                Thread.Sleep(30);
                var text = FlySqlExtensions.EmitSql("Hello", null);
                Assert.Equal("Hello2", text);
            }
            finally
            {
                File.Delete(tempFile);
                Directory.Delete("sqls");
            }
        }
    }
}
