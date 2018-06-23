namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class SqlParameter : SqlNameSyntax
    {
        public SqlParameter(RawToken at, SqlIdentifierName identifier)
            : base(ImmutableArray.Create<SqlNode>(identifier))
        {
            this.At = at.WithParent(this);
            this.Identifier = identifier;
        }

        public SqlToken At { get; }

        public SqlExpression Identifier { get; }
    }
}
