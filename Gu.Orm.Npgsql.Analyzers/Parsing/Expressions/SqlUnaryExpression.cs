namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class SqlUnaryExpression : SqlExpression
    {
        public SqlUnaryExpression(string sql, RawToken @operator, SqlExpression operand)
            : base(sql, ImmutableArray.Create<SqlNode>(operand))
        {
            this.Operator = @operator.WithParent(this);
            this.Operand = operand;
        }

        public SqlToken Operator { get; }

        public SqlExpression Operand { get; }
    }
}
