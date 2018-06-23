namespace Gu.Orm.Npgsql.Analyzers.Parsing.Clauses
{
    using System.Collections.Immutable;

    public class WhereClause : SqlNode
    {
        protected WhereClause(SqlBinaryExpression condition)
            : base(ImmutableArray.Create<SqlNode>(condition))
        {
            this.Condition = condition;
        }

        public SqlBinaryExpression Condition { get; }
    }
}
