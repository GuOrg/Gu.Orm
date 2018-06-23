namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public abstract class SqlNode
    {
        protected readonly string Sql;

        protected SqlNode(string sql, ImmutableArray<SqlNode> children)
        {
            this.Sql = sql;
            foreach (var child in children)
            {
                child.Parent = this;
            }

            this.Children = children;
        }

        public ImmutableArray<SqlNode> Children { get; }

        public SqlNode Parent { get; private set; }

        public abstract string ToDisplayString();

        protected static ImmutableArray<SqlNode> CreateChildren(SqlNode node)
        {
            return node == null ? ImmutableArray<SqlNode>.Empty : ImmutableArray.Create(node);
        }

        protected static ImmutableArray<SqlNode> CreateChildren(SqlNode node1, SqlNode node2)
        {
            if (node1 == null)
            {
                return node2 == null ? ImmutableArray<SqlNode>.Empty : ImmutableArray.Create(node2);
            }

            if (node2 == null)
            {
                return ImmutableArray.Create(node1);
            }

            return ImmutableArray.Create(node1, node2);
        }

        protected static ImmutableArray<SqlNode> CreateChildren(SqlNode node1, SqlNode node2, SqlNode node3)
        {
            if (node1 == null)
            {
                return CreateChildren(node2, node3);
            }

            if (node2 == null)
            {
                return CreateChildren(node1, node3);
            }

            if (node3 == null)
            {
                return CreateChildren(node1, node2);
            }

            return ImmutableArray.Create(node1, node2, node3);
        }
    }
}
