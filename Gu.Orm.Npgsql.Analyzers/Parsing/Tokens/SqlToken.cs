namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public struct SqlToken
    {
        public SqlToken(SqlKind kind, int start, int end, SqlNode parent)
        {
            this.Parent = parent;
            this.Kind = kind;
            this.Start = start;
            this.End = end;
        }

        public SqlNode Parent { get; }

        public SqlKind Kind { get; }

        public int Start { get; }

        public int End { get; }

        public int Length => this.End - this.Start;

        public string ToDisplayString(string sql)
        {
            if (this.Start < 0)
            {
                return string.Empty;
            }

            return sql.Substring(this.Start, this.Length);
        }
    }
}
