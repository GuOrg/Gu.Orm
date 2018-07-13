namespace Gu.Orm.Npgsql.Analyzers.Parsing.Clauses
{
    public class WhereClause : SqlNode
    {
        public WhereClause(string sql, RawToken keyword, SqlBinaryExpression condition)
            : base(sql, CreateChildren(condition))
        {
            this.Keyword = keyword.WithParent(this);
            this.Condition = condition;
        }

        public SqlToken Keyword { get; }

        public SqlBinaryExpression Condition { get; }

        public override bool IsValid => this.Keyword.Kind == SqlKind.WhereKeyword &&
                                        this.Condition?.IsValid == true;

        public override string ToDisplayString() => $"{this.Keyword.ToDisplayString(this.Sql)} {this.Condition.ToDisplayString()}";
    }
}
