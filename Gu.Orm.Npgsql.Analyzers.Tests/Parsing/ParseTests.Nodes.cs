namespace Gu.Orm.Npgsql.Analyzers.Tests.Parsing
{
    using Gu.Orm.Npgsql.Analyzers.Parsing;
    using NUnit.Framework;

    public partial class ParseTests
    {
        public class Nodes
        {
            [TestCase("*")]
            [TestCase("bar")]
            [TestCase("bar AS meh")]
            [TestCase("\"Bar\"")]
            [TestCase("foo.bar")]
            [TestCase("foo.\"Bar\"")]
            [TestCase("foo.bar AS meh")]
            [TestCase("foo.\"Bar\" AS meh")]
            public void ColumnRef(string sql)
            {
                var node = Parse.ColumnRef(sql);
                Assert.AreEqual(sql, node.ToString());
            }
        }
    }
}
