﻿namespace Gu.Orm.Npgsql.Analyzers.Tests.Parsing
{
    using System.Linq;
    using Gu.Orm.Npgsql.Analyzers.Parsing;
    using NUnit.Framework;

    public partial class ParseTests
    {
        public class Tokens
        {
            [TestCase("SELECT 1", "Identifier: SELECT Integer: 1")]
            [TestCase("-- SELECT 'abc'", "Comment: -- SELECT 'abc'")]
            [TestCase("-- SELECT 'abc'\r\nSELECT 1", "Comment: -- SELECT 'abc' Identifier: SELECT Integer: 1")]
            [TestCase("/*\r\n-- SELECT 'abc'\r\n*/\r\nSELECT 1", "Comment: /*\r\n-- SELECT 'abc'\r\n*/ Identifier: SELECT Integer: 1")]
            [TestCase("SELECT 'abc'", "Identifier: SELECT String: 'abc'")]
            [TestCase("SELECT 1.2", "Identifier: SELECT Float: 1.2")]
            [TestCase("SELECT \"Bar\" FROM foo", "Identifier: SELECT Identifier: \"Bar\" Identifier: FROM Identifier: foo")]
            [TestCase("SELECT * FROM foos", "Identifier: SELECT Star: * Identifier: FROM Identifier: foos")]
            [TestCase("select f.bar, f.baz from foos f", "Identifier: select Identifier: f Point: . Identifier: bar Comma: , Identifier: f Point: . Identifier: baz Identifier: from Identifier: foos Identifier: f")]
            [TestCase("select * from t match_recognize (  order by run_date  pattern ( anything )   define    anything as run_date = run_date);", "Identifier: select Star: * Identifier: from Identifier: t Identifier: match_recognize OpenParens: ( Identifier: order Identifier: by Identifier: run_date Identifier: pattern OpenParens: ( Identifier: anything CloseParens: ) Identifier: define Identifier: anything Identifier: as Identifier: run_date Equal: = Identifier: run_date CloseParens: ) Semicolon: ;")]
            public void Valid(string sql, string expected)
            {
                var tokens = Parse.Tokens(sql);
                var actual = string.Join(" ", tokens.Select(x => TypeAndValue(sql, x)));
                CollectionAssert.AreEqual(expected, actual);
            }

            private static string TypeAndValue(string sql, SqlToken sqlToken)
            {
                return $"{sqlToken.Kind}: {sql.Substring(sqlToken.Start, sqlToken.Length)}";
            }
        }
    }
}
