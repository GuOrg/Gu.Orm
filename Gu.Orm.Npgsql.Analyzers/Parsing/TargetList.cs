namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;
    using System.Linq;

    public class TargetList : SqlNode
    {
        public TargetList(string sql, ImmutableArray<ResTarget> targets)
            : base(sql, targets.As<SqlNode>())
        {
            this.Targets = targets;
        }

        public ImmutableArray<ResTarget> Targets { get; }

        public override string ToDisplayString()
        {
            return string.Join(", ", this.Targets.Select(x => x.ToDisplayString()));
        }
    }
}
