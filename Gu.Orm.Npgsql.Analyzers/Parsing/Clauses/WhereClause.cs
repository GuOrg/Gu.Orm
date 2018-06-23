namespace Gu.Orm.Npgsql.Analyzers.Parsing.Clauses
{
    public class WhereClause : SqlNode
    {
        protected WhereClause(string sql, SqlBinaryExpression condition)
            : base(sql, CreateChildren(condition))
        {
            this.Condition = condition;
        }

        public SqlBinaryExpression Condition { get; }

        public override string ToDisplayString() => this.Condition.ToDisplayString();
    }
}
