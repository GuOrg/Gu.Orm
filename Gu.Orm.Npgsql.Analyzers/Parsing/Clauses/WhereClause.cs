namespace Gu.Orm.Npgsql.Analyzers.Parsing.Clauses
{
    using System.Collections.Immutable;

    public class WhereClause : SqlNode
    {
        protected WhereClause(string sql, SqlBinaryExpression condition)
            : base(sql, ImmutableArray.Create<SqlNode>(condition))
        {
            this.Condition = condition;
        }

        public SqlBinaryExpression Condition { get; }
    }
}
