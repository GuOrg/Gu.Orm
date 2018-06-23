namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public struct RawToken
    {
        public RawToken(SqlKind kind, int start, int end)
        {
            this.Kind = kind;
            this.Start = start;
            this.End = end;
        }

        public static RawToken None { get; } = new RawToken(SqlKind.None, -1, -1);

        public SqlKind Kind { get; }

        public int Start { get; }

        public int End { get; }

        public int Length => this.End - this.Start;

        public static RawToken SingleChar(SqlKind type, int position) => new RawToken(type, position, position + 1);

        public SqlToken WithParent(SqlNode node) => new SqlToken(this.Kind, this.Start, this.End, node);
    }
}