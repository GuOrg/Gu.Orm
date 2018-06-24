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
            [TestCase("1 + 2", "Integer: 1 Add: + Integer: 2")]
            [TestCase("1 - 2", "Integer: 1 Subtract: - Integer: 2")]
            [TestCase("1 * 2", "Integer: 1 Multiply: * Integer: 2")]
            [TestCase("1 / 2", "Integer: 1 Divide: / Integer: 2")]
            [TestCase("1 % 2", "Integer: 1 Modulo: % Integer: 2")]
            [TestCase("1 ^ 2", "Integer: 1 Exponent: ^ Integer: 2")]
            [TestCase("1 |/ 2", "Integer: 1 SquareRoot: |/ Integer: 2")]
            [TestCase("1 ||/ 2", "Integer: 1 CubeRoot: ||/ Integer: 2")]
            [TestCase("1 ! 2", "Integer: 1 Factorial: ! Integer: 2")]
            [TestCase("!! 2", "FactorialPrefix: !! Integer: 2")]
            [TestCase("@ 2", "Abs: @ Integer: 2")]
            [TestCase("1 & 2", "Integer: 1 BitwiseAnd: & Integer: 2")]
            [TestCase("1 | 2", "Integer: 1 BitwiseOr: | Integer: 2")]
            [TestCase("1 # 2", "Integer: 1 BitwiseXor: # Integer: 2")]
            [TestCase("~ 2", "BitwiseNot: ~ Integer: 2")]
            [TestCase("1 <> 2", "Integer: 1 NotEqual: <> Integer: 2")]
            [TestCase("1 << 2", "Integer: 1 BitwiseShiftLeft: << Integer: 2")]
            [TestCase("1 >> 2", "Integer: 1 BitwiseShiftRight: >> Integer: 2")]
            [TestCase("-- SELECT 'abc'", "Comment: -- SELECT 'abc'")]
            [TestCase("-- SELECT 'abc'\r\nSELECT 1", "Comment: -- SELECT 'abc' Identifier: SELECT Integer: 1")]
            [TestCase("/*\r\n-- SELECT 'abc'\r\n*/\r\nSELECT 1", "BlockComment: /*\r\n-- SELECT 'abc'\r\n*/ Identifier: SELECT Integer: 1")]
            [TestCase("SELECT 'abc'", "Identifier: SELECT String: 'abc'")]
            [TestCase("SELECT 1.2", "Identifier: SELECT Float: 1.2")]
            [TestCase("SELECT bar FROM foo", "Identifier: SELECT Identifier: bar Identifier: FROM Identifier: foo")]
            [TestCase("SELECT bar FROM\r\n foo", "Identifier: SELECT Identifier: bar Identifier: FROM Identifier: foo")]
            [TestCase("SELECT \"Bar\" FROM foo", "Identifier: SELECT QuotedIdentifier: \"Bar\" Identifier: FROM Identifier: foo")]
            [TestCase("SELECT \"Bar\"\r\nFROM foo", "Identifier: SELECT QuotedIdentifier: \"Bar\" Identifier: FROM Identifier: foo")]
            [TestCase("SELECT * FROM foos", "Identifier: SELECT Multiply: * Identifier: FROM Identifier: foos")]
            [TestCase("select f.bar, f.baz from foos f", "Identifier: select Identifier: f Point: . Identifier: bar Comma: , Identifier: f Point: . Identifier: baz Identifier: from Identifier: foos Identifier: f")]
            [TestCase("select * from t match_recognize (  order by run_date  pattern ( anything )   define    anything as run_date = run_date);", "Identifier: select Multiply: * Identifier: from Identifier: t Identifier: match_recognize OpenParen: ( Identifier: order Identifier: by Identifier: run_date Identifier: pattern OpenParen: ( Identifier: anything CloseParen: ) Identifier: define Identifier: anything Identifier: as Identifier: run_date Equal: = Identifier: run_date CloseParen: ) Semicolon: ;")]
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
