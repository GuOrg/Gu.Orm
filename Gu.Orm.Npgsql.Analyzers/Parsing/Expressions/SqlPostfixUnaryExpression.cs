namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class SqlPostfixUnaryExpression : SqlExpression
    {
        public SqlPostfixUnaryExpression(string sql, SqlExpression operand, RawToken @operator)
            : base(sql, CreateChildren(operand))
        {
            this.Operand = operand;
            this.Operator = @operator.WithParent(this);
        }

        public SqlExpression Operand { get; }

        public SqlToken Operator { get; }

        public override bool IsValid => this.Operator.Kind != SqlKind.Unknown &&
                                        this.Operand?.IsValid == true;

        public override string ToDisplayString() => $"{this.Operand.ToDisplayString()} {this.Operator.ToDisplayString(this.Sql)}";
    }
}