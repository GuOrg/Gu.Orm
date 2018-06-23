namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System;
    using System.Collections.Immutable;

    public static partial class Parse
    {
        public static ColumnRef ParseColumnRef(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return ParseColumnRef(tokens, ref position);
        }

        private static ColumnRef ParseColumnRef(ImmutableArray<RawToken> tokens, ref int position)
        {
            throw new NotImplementedException();
        }
    }
}
