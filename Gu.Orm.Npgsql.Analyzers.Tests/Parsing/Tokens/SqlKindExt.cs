namespace Gu.Orm.Npgsql.Analyzers.Tests.Parsing.Tokens
{
    using System;
    using System.Linq;
    using Gu.Orm.Npgsql.Analyzers.Parsing;
    using NUnit.Framework;

    public class SqlKindExt
    {
        private static readonly TestCaseData[] BinaryOperators =
        {
            new TestCaseData(SqlKind.Unknown, false),
            new TestCaseData(SqlKind.Identifier, false),
            new TestCaseData(SqlKind.QuotedIdentifier, false),
            new TestCaseData(SqlKind.String, false),
            new TestCaseData(SqlKind.Integer, false),
            new TestCaseData(SqlKind.Float, false),
            new TestCaseData(SqlKind.ExclamationExclamationToken, false),
            new TestCaseData(SqlKind.AtToken, false),
            new TestCaseData(SqlKind.CommaToken, false),
            new TestCaseData(SqlKind.DotToken, false),
            new TestCaseData(SqlKind.SemicolonToken, false),
            new TestCaseData(SqlKind.ColonToken, false),
            new TestCaseData(SqlKind.OpenParenToken, false),
            new TestCaseData(SqlKind.CloseParenToken, false),
            new TestCaseData(SqlKind.OpenBracketToken, false),
            new TestCaseData(SqlKind.CloseBracketToken, false),
            new TestCaseData(SqlKind.Comment, false),
            new TestCaseData(SqlKind.BlockComment, false),
            new TestCaseData(SqlKind.None, false),
            new TestCaseData(SqlKind.PlusToken, true),
            new TestCaseData(SqlKind.MinusToken, true),
            new TestCaseData(SqlKind.AsteriskToken, true),
            new TestCaseData(SqlKind.SlashToken, true),
            new TestCaseData(SqlKind.ExponentToken, true),
            new TestCaseData(SqlKind.PercentToken, true),
            new TestCaseData(SqlKind.SquareRootToken, true),
            new TestCaseData(SqlKind.CubeRootToken, true),
            new TestCaseData(SqlKind.ExclamationToken, true),
            new TestCaseData(SqlKind.AmpersandToken, true),
            new TestCaseData(SqlKind.BarToken, true),
            new TestCaseData(SqlKind.HashToken, true),
            new TestCaseData(SqlKind.TildeToken, true),
            new TestCaseData(SqlKind.LessThanLessThanToken, true),
            new TestCaseData(SqlKind.GreaterThanGreaterThanToken, true),
            new TestCaseData(SqlKind.EqualsToken, true),
            new TestCaseData(SqlKind.NotEqualToken, true),
            new TestCaseData(SqlKind.LessThanToken, true),
            new TestCaseData(SqlKind.LessThanEqualsToken, true),
            new TestCaseData(SqlKind.GreaterThanToken, true),
            new TestCaseData(SqlKind.GreaterThanEqualsToken, true),
        };

        [TestCaseSource(nameof(BinaryOperators))]
        public void IsBinaryOperator(SqlKind kind, bool expected)
        {
            Assert.AreEqual(expected, kind.IsBinaryOperator());
        }

        [Test]
        public void AllMembersHasTestCases()
        {
            CollectionAssert.AreEquivalent(Enum.GetValues(typeof(SqlKind)), BinaryOperators.Select(x => x.Arguments[0]));
        }
    }
}
