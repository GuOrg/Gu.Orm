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
            new TestCaseData(SqlKind.FactorialPrefix, false),
            new TestCaseData(SqlKind.Abs, false),
            new TestCaseData(SqlKind.Comma, false),
            new TestCaseData(SqlKind.Point, false),
            new TestCaseData(SqlKind.Semicolon, false),
            new TestCaseData(SqlKind.Colon, false),
            new TestCaseData(SqlKind.OpenParen, false),
            new TestCaseData(SqlKind.CloseParen, false),
            new TestCaseData(SqlKind.OpenBracket, false),
            new TestCaseData(SqlKind.CloseBracket, false),
            new TestCaseData(SqlKind.Comment, false),
            new TestCaseData(SqlKind.BlockComment, false),
            new TestCaseData(SqlKind.None, false),
            new TestCaseData(SqlKind.Add, true),
            new TestCaseData(SqlKind.Subtract, true),
            new TestCaseData(SqlKind.Multiply, true),
            new TestCaseData(SqlKind.Divide, true),
            new TestCaseData(SqlKind.Exponent, true),
            new TestCaseData(SqlKind.Modulo, true),
            new TestCaseData(SqlKind.SquareRoot, true),
            new TestCaseData(SqlKind.CubeRoot, true),
            new TestCaseData(SqlKind.Factorial, true),
            new TestCaseData(SqlKind.BitwiseAnd, true),
            new TestCaseData(SqlKind.BitwiseOr, true),
            new TestCaseData(SqlKind.BitwiseXor, true),
            new TestCaseData(SqlKind.BitwiseNot, true),
            new TestCaseData(SqlKind.BitwiseShiftLeft, true),
            new TestCaseData(SqlKind.BitwiseShiftRight, true),
            new TestCaseData(SqlKind.Equal, true),
            new TestCaseData(SqlKind.NotEqual, true),
            new TestCaseData(SqlKind.LessThan, true),
            new TestCaseData(SqlKind.LessThanOrEqual, true),
            new TestCaseData(SqlKind.GreaterThan, true),
            new TestCaseData(SqlKind.GreaterThanOrEqual, true),
        };

        [TestCaseSource(nameof(BinaryOperators))]
        public void Test(SqlKind kind, bool expected)
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
