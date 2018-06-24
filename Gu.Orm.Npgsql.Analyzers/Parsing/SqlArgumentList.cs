namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;
    using System.Linq;

    public class SqlArgumentList : SqlNode
    {
        public SqlArgumentList(string sql, ImmutableArray<SqlArgument> arguments)
            : base(sql, arguments.As<SqlNode>())
        {
            this.Arguments = arguments;
        }

        public ImmutableArray<SqlArgument> Arguments { get; }

        public override string ToDisplayString()
        {
            return string.Join(", ", this.Arguments.Select(x => x.ToDisplayString()));
        }
    }
}
