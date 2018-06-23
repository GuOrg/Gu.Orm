namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class SqlUnaryExpression : SqlExpression
    {
        public SqlUnaryExpression(string sql, RawToken @operator, SqlExpression operand)
            : base(sql, CreateChildren(operand))
        {
            this.Operator = @operator.WithParent(this);
            this.Operand = operand;
        }

        public SqlToken Operator { get; }

        public SqlExpression Operand { get; }

        public override string ToDisplayString() => $"{this.Operator.ToDisplayString(this.Sql)} {this.Operand.ToDisplayString()}";
    }
}
