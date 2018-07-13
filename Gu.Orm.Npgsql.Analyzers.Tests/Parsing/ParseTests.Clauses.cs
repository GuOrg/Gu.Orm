namespace Gu.Orm.Npgsql.Analyzers.Tests.Parsing
{
    using Gu.Orm.Npgsql.Analyzers.Parsing;
    using NUnit.Framework;

    public partial class ParseTests
    {
        public class Clauses
        {
            [TestCase("FROM B")]
            [TestCase("FROM B b")]
            public void From(string sql)
            {
                var node = Parse.FromClause(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                Assert.AreEqual(true, node.IsValid);
                SqlNodeAssert.Tree(node);
            }

            [TestCase("WHERE id = 1")]
            public void Where(string sql)
            {
                var node = Parse.WhereClause(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                Assert.AreEqual(true, node.IsValid);
                SqlNodeAssert.Tree(node);
            }
        }
    }
}
