namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class SqlBinaryExpression : SqlExpression
    {
        public SqlBinaryExpression(string sql, SqlExpression left, RawToken @operator, SqlExpression right)
            : base(sql, CreateChildren(left, right))
        {
            this.Left = left;
            this.Operator = @operator.WithParent(this);
            this.Right = right;
        }

        public SqlExpression Left { get; }

        public SqlToken Operator { get; }

        public SqlExpression Right { get; }

        public override string ToDisplayString()
        {
            return $"{this.Left.ToDisplayString()} {this.Operator.ToDisplayString(this.Sql)} {this.Right.ToDisplayString()}";
        }
    }
}
