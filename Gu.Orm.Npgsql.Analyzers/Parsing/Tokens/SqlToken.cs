namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public struct SqlToken
    {
        public SqlToken(SqlKind kind, int start, int end)
        {
            this.Kind = kind;
            this.Start = start;
            this.End = end;
        }

        public static SqlToken None { get; } = new SqlToken(SqlKind.None, -1, -1);

        public SqlKind Kind { get; }

        public int Start { get; }

        public int End { get; }

        public int Length => this.End - this.Start;

        public static SqlToken SingleChar(SqlKind type, int position) => new SqlToken(type, position, position + 1);
    }
}