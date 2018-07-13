namespace Gu.Orm.Npgsql.Analyzers.Tests.Parsing
{
    using Gu.Orm.Npgsql.Analyzers.Parsing;
    using NUnit.Framework;

    public partial class ParseTests
    {
        public class Nodes
        {
            [TestCase("*")]
            [TestCase("1")]
            [TestCase("1, 2")]
            [TestCase("foo")]
            [TestCase("foo, bar")]
            [TestCase("bar AS meh")]
            [TestCase("\"Bar\"")]
            [TestCase("\"Bar\" AS \"Meh\"")]
            [TestCase("foo.bar")]
            [TestCase("foo.\"Bar\"")]
            [TestCase("foo.bar AS meh")]
            [TestCase("foo.\"Bar\" AS meh")]
            [TestCase("foo.\"Bar1\" AS meh1, foo.\"Bar2\" AS meh2")]
            [TestCase("foo.\"Bar\" AS \"Meh\"")]
            [TestCase("foo.\"Bar1\" AS \"Meh1\", foo.\"Bar2\" AS \"Meh2\"")]
            public void TargetList(string sql)
            {
                var node = Parse.TargetList(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                Assert.AreEqual(true, node.IsValid);
                SqlNodeAssert.Tree(node);
            }

            [TestCase("*")]
            [TestCase("1")]
            [TestCase("bar")]
            [TestCase("bar AS meh")]
            [TestCase("\"Bar\"")]
            [TestCase("\"Bar\" AS \"Meh\"")]
            [TestCase("foo.bar")]
            [TestCase("foo.\"Bar\"")]
            [TestCase("foo.bar AS meh")]
            [TestCase("foo.\"Bar\" AS meh")]
            [TestCase("foo.\"Bar\" AS \"Meh\"")]
            [TestCase("UPPER(word)")]
            [TestCase("UPPER(word) AS upper_word")]
            [TestCase("UPPER(word) AS SELECT")]
            [TestCase("UPPER(word) AS \"SELECT\"")]
            [TestCase("UPPER(CONCAT('a', 'b'))")]
            [TestCase("UPPER(CONCAT('a', 'b')) as meh")]
            [TestCase("UPPER(CONCAT('a', 'b', UPPER('c')))")]
            [TestCase("UPPER(CONCAT('a', 'b', UPPER('c'))) as meh")]
            public void ResTarget(string sql)
            {
                var node = Parse.ResTarget(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                Assert.AreEqual(true, node.IsValid);
                SqlNodeAssert.Tree(node);
            }

            [TestCase("foo")]
            [TestCase("foo f")]
            [TestCase("foo.bar")]
            [TestCase("foo.bar b")]
            [TestCase("foo.\"Bar\"")]
            [TestCase("foo.\"Bar\" b")]
            public void RangeVar(string sql)
            {
                var node = Parse.RangeVar(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                Assert.AreEqual(true, node.IsValid);
                SqlNodeAssert.Tree(node);
            }

            [TestCase("foo WHERE", "foo")]
            [TestCase("foo f WHERE", "foo f")]
            [TestCase("foo.\"Bar\" b WHERE", "foo.\"Bar\" b")]
            public void RangeVarEnd(string sql, string expected)
            {
                var node = Parse.RangeVar(sql);
                Assert.AreEqual(expected, node.ToDisplayString());
                Assert.AreEqual(true, node.IsValid);
                SqlNodeAssert.Tree(node);
            }

            [TestCase("*")]
            [TestCase("bar")]
            [TestCase("\"Bar\"")]
            [TestCase("foo.bar")]
            [TestCase("foo.\"Bar\"")]
            public void ColumnRef(string sql)
            {
                var node = Parse.ColumnRef(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                Assert.AreEqual(true, node.IsValid);
                SqlNodeAssert.Tree(node);
            }

            [TestCase("foo.")]
            public void ColumnRefInvalid(string sql)
            {
                var node = Parse.ColumnRef(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                Assert.AreEqual(false, node.IsValid);
                SqlNodeAssert.Tree(node);
            }

            [TestCase("1")]
            [TestCase("1.2")]
            [TestCase("'abc'")]
            public void Literal(string sql)
            {
                var node = Parse.Literal(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                Assert.AreEqual(true, node.IsValid);
                SqlNodeAssert.Tree(node);
            }

            [TestCase("a = b")]
            [TestCase("a <> b")]
            [TestCase("a < b")]
            [TestCase("a <= b")]
            [TestCase("a > b")]
            [TestCase("a >= b")]
            [TestCase("a + b")]
            [TestCase("a - b")]
            [TestCase("a / b")]
            [TestCase("a % b")]
            [TestCase("a ^ b")]
            [TestCase("a * b")]
            [TestCase("a |/ b")]
            [TestCase("a ||/ b")]
            [TestCase("a ! b")]
            [TestCase("a & b")]
            [TestCase("a | b")]
            [TestCase("a ~ b")]
            [TestCase("a # b")]
            [TestCase("a << b")]
            [TestCase("a >> b")]
            [TestCase("a AND b")]
            [TestCase("a OR b")]
            [TestCase("(1 + 2) < (2 + 3)")]
            public void BinaryExpression(string sql)
            {
                var node = Parse.BinaryExpression(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                Assert.AreEqual(true, node.IsValid);
                SqlNodeAssert.Tree(node);
            }

            [TestCase("a = b AND c <> d", "a = b", "c <> d")]
            [TestCase("a OR b AND c", "a", "b AND c")]
            [TestCase("a AND b OR c", "a AND b", "c")]
            [TestCase("(a OR b) AND c", "(a OR b)", "c")]
            public void BinaryExpressionPrecedence(string sql, string left, string right)
            {
                var node = Parse.BinaryExpression(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                Assert.AreEqual(left, node.Left.ToDisplayString());
                Assert.AreEqual(right, node.Right.ToDisplayString());
                Assert.AreEqual(true, node.IsValid);
                SqlNodeAssert.Tree(node);
            }

            [TestCase("- 1")]
            [TestCase("+ 1")]
            [TestCase("!! 1")]
            [TestCase("~ 1")]
            [TestCase("@ a")]
            [TestCase("NOT a")]
            public void PrefixUnaryExpression(string sql)
            {
                var node = Parse.PrefixUnaryExpression(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                Assert.AreEqual(true, node.IsValid);
                SqlNodeAssert.Tree(node);
            }

            [TestCase("(1)")]
            [TestCase("(a + b)")]
            public void ParenthesizedExpression(string sql)
            {
                var node = Parse.ParenthesizedExpression(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                Assert.AreEqual(true, node.IsValid);
                SqlNodeAssert.Tree(node);
            }

            [TestCase("NOW()")]
            [TestCase("MAX(foo)")]
            [TestCase("MAX(foo.bar)")]
            [TestCase("MOD(1, 2)")]
            [TestCase("UPPER(CONCAT('a', 'b'))")]
            [TestCase("UPPER(CONCAT('a', 'b', UPPER('c')))")]
            [TestCase("CONCAT(1 <> 2, 1 = 2)")]
            public void Invocation(string sql)
            {
                var node = Parse.Invocation(sql);
                Assert.AreEqual(sql, node.ToDisplayString());
                Assert.AreEqual(true, node.IsValid);
                SqlNodeAssert.Tree(node);
            }

            [TestCase("MOD(1,, 2)", "MOD(1")]
            public void InvocationInvalid(string sql, string expected)
            {
                var node = Parse.Invocation(sql);
                Assert.AreEqual(expected, node.ToDisplayString());
                Assert.AreEqual(false, node.IsValid);
                SqlNodeAssert.Tree(node);
            }
        }
    }
}
