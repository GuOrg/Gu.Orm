namespace Gu.Orm.Npgsql.Analyzers.Tests.Parsing
{
    using System.Linq;
    using Gu.Orm.Npgsql.Analyzers.Parsing;
    using NUnit.Framework;

    public static class SqlNodeAssert
    {
        public static void Tree(SqlNode node)
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
                    Tree(child);
                }
            }
        }
    }
}