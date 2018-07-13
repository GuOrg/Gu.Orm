namespace Gu.Orm.Npgsql.Analyzers.Tests.Parsing
{
    using System.Linq;
    using Gu.Orm.Npgsql.Analyzers.Parsing;
    using NUnit.Framework;

    public partial class ParseTests
    {
        public class Tokens
        {
            [TestCase("SELECT 1", "Identifier: SELECT Integer: 1")]
            [TestCase("1 + 2", "Integer: 1 PlusToken: + Integer: 2")]
            [TestCase("1 - 2", "Integer: 1 MinusToken: - Integer: 2")]
            [TestCase("1 * 2", "Integer: 1 AsteriskToken: * Integer: 2")]
            [TestCase("1 / 2", "Integer: 1 SlashToken: / Integer: 2")]
            [TestCase("1 % 2", "Integer: 1 PercentToken: % Integer: 2")]
            [TestCase("1 ^ 2", "Integer: 1 ExponentToken: ^ Integer: 2")]
            [TestCase("1 |/ 2", "Integer: 1 SquareRootToken: |/ Integer: 2")]
            [TestCase("1 ||/ 2", "Integer: 1 CubeRootToken: ||/ Integer: 2")]
            [TestCase("1 ! 2", "Integer: 1 ExclamationToken: ! Integer: 2")]
            [TestCase("!! 2", "ExclamationExclamationToken: !! Integer: 2")]
            [TestCase("@ 2", "AtToken: @ Integer: 2")]
            [TestCase("1 & 2", "Integer: 1 AmpersandToken: & Integer: 2")]
            [TestCase("1 | 2", "Integer: 1 BarToken: | Integer: 2")]
            [TestCase("1 # 2", "Integer: 1 HashToken: # Integer: 2")]
            [TestCase("~ 2", "TildeToken: ~ Integer: 2")]
            [TestCase("1 <> 2", "Integer: 1 NotEqualsToken: <> Integer: 2")]
            [TestCase("1 << 2", "Integer: 1 LessThanLessThanToken: << Integer: 2")]
            [TestCase("1 >> 2", "Integer: 1 GreaterThanGreaterThanToken: >> Integer: 2")]
            [TestCase("-- SELECT 'abc'", "Comment: -- SELECT 'abc'")]
            [TestCase("-- SELECT 'abc'\r\nSELECT 1", "Comment: -- SELECT 'abc' Identifier: SELECT Integer: 1")]
            [TestCase("/*\r\n-- SELECT 'abc'\r\n*/\r\nSELECT 1", "BlockComment: /*\r\n-- SELECT 'abc'\r\n*/ Identifier: SELECT Integer: 1")]
            [TestCase("SELECT 'abc'", "Identifier: SELECT String: 'abc'")]
            [TestCase("SELECT 1.2", "Identifier: SELECT Float: 1.2")]
            [TestCase("SELECT bar FROM foo", "Identifier: SELECT Identifier: bar Identifier: FROM Identifier: foo")]
            [TestCase("SELECT bar FROM\r\n foo", "Identifier: SELECT Identifier: bar Identifier: FROM Identifier: foo")]
            [TestCase("SELECT \"Bar\" FROM foo", "Identifier: SELECT QuotedIdentifier: \"Bar\" Identifier: FROM Identifier: foo")]
            [TestCase("SELECT \"Bar\"\r\nFROM foo", "Identifier: SELECT QuotedIdentifier: \"Bar\" Identifier: FROM Identifier: foo")]
            [TestCase("SELECT * FROM foos", "Identifier: SELECT AsteriskToken: * Identifier: FROM Identifier: foos")]
            [TestCase("select f.bar, f.baz from foos f", "Identifier: select Identifier: f DotToken: . Identifier: bar CommaToken: , Identifier: f DotToken: . Identifier: baz Identifier: from Identifier: foos Identifier: f")]
            [TestCase("select * from t match_recognize (  order by run_date  pattern ( anything )   define    anything as run_date = run_date);", "Identifier: select AsteriskToken: * Identifier: from Identifier: t Identifier: match_recognize OpenParenToken: ( Identifier: order Identifier: by Identifier: run_date Identifier: pattern OpenParenToken: ( Identifier: anything CloseParenToken: ) Identifier: define Identifier: anything Identifier: as Identifier: run_date EqualsToken: = Identifier: run_date CloseParenToken: ) SemicolonToken: ;")]
            public void Valid(string sql, string expected)
            {
                var tokens = Parse.Tokens(sql);
                var actual = string.Join(" ", tokens.Select(x => TypeAndValue(sql, x)));
                CollectionAssert.AreEqual(expected, actual);
            }

            private static string TypeAndValue(string sql, RawToken token)
            {
                return $"{token.Kind}: {sql.Substring(token.Start, token.Length)}";
            }
        }
    }
}
