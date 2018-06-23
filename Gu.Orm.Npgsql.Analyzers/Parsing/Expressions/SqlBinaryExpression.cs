namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class SqlBinaryExpression : SqlExpression
    {
        public SqlBinaryExpression(SqlExpression left, RawToken @operator, SqlExpression right)
            : base(ImmutableArray.Create<SqlNode>(left, right))
        {
            this.Left = left;
            this.Operator = @operator.WithParent(this);
            this.Right = right;
        }

        public SqlExpression Left { get; }

        public SqlToken Operator { get; }

        public SqlExpression Right { get; }
    }
}
