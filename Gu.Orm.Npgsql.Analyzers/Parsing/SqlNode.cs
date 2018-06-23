namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public abstract class SqlNode
    {
        protected SqlNode(ImmutableArray<SqlNode> children)
        {
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
