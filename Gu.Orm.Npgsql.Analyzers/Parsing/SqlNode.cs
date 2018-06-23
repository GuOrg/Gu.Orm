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
    }
}
