namespace Gu.Orm.Npgsql.Analyzers.Parsing.Clauses
{
    public class FromClause : SqlNode
    {
        public FromClause(string sql, RawToken keyword, RangeVar rangeVar)
            : base(sql, CreateChildren(rangeVar))
        {
            this.Keyword = keyword.WithParent(this);
            this.RangeVar = rangeVar;
        }

        public SqlToken Keyword { get; }

        public RangeVar RangeVar { get; }

        public override bool IsValid => this.Keyword.Kind == SqlKind.FromKeyword &&
                                        this.RangeVar?.IsValid == true;

        public override string ToDisplayString() => $"{this.Keyword.ToDisplayString(this.Sql)} {this.RangeVar.ToDisplayString()}";
    }
}
