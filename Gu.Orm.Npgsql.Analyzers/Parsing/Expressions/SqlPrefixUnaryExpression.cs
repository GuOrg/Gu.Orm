namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class SqlPrefixUnaryExpression : SqlExpression
    {
        public SqlPrefixUnaryExpression(string sql, RawToken @operator, SqlExpression operand)
            : base(sql, CreateChildren(operand))
        {
            this.Operator = @operator.WithParent(this);
            this.Operand = operand;
        }

        public SqlToken Operator { get; }

        public SqlExpression Operand { get; }

        public override bool IsValid => this.Operator.Kind != SqlKind.Unknown &&
                                        this.Operand?.IsValid == true;

        public override string ToDisplayString() => $"{this.Operator.ToDisplayString(this.Sql)} {this.Operand.ToDisplayString()}";
    }
}
