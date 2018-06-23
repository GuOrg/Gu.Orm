namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class ResTarget : SqlNode
    {
        public ResTarget(string sql, SqlIdentifierName name, ColumnRef value)
            : base(sql, ImmutableArray.Create<SqlNode>(name, value))
        {
            this.Name = name;
            this.Value = value;
        }

        public SqlIdentifierName Name { get; }

        public ColumnRef Value { get; }
    }
}
