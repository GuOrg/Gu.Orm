namespace Gu.Orm.Npgsql.Analyzers.Tests.Parsing
{
    using System.Linq;
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
            [TestCase("\"Bar\" AS \"Meh\"")]
            [TestCase("foo.bar")]
            [TestCase("foo.\"Bar\"")]
            [TestCase("foo.bar AS meh")]
            [TestCase("foo.\"Bar\" AS meh")]
            [TestCase("foo.\"Bar\" AS \"Meh\"")]
            public void ResTarget(string sql)
            {
                var node = Parse.ResTarget(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                AssertTree(node);
            }

            [TestCase("*")]
            [TestCase("bar")]
            [TestCase("\"Bar\"")]
            [TestCase("foo.bar")]
            [TestCase("foo.\"Bar\"")]
            public void ColumnRef(string sql)
            {
                var node = Parse.ColumnRef(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                AssertTree(node);
            }

            private static void AssertTree(SqlNode node)
            {
                foreach (var property in node.GetType().GetProperties().Where(x => x.PropertyType == typeof(SqlToken)))
                {
                    var token = (SqlToken)property.GetValue(node);
                    Assert.AreSame(node, token.Parent);
                }

                foreach (var property in node.GetType()
                                             .GetProperties()
                                             .Where(x => typeof(SqlNode).IsAssignableFrom(x.PropertyType))
                                             .Where(x => x.Name != nameof(SqlNode.Parent)))
                {
                    var child = (SqlNode)property.GetValue(node);
                    if (child != null)
                    {
                        Assert.AreSame(node, child.Parent);
                        AssertTree(child);
                    }
                }
            }
        }
    }
}
