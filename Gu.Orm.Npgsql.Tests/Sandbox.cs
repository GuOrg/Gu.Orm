namespace Gu.Orm.Npgsql.Tests
{
    using System;
    using System.Configuration;
    using System.Data;
    using global::Npgsql;
    using NUnit.Framework;

    public class Sandbox
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
            {
                db.Open();
                using (var command = db.CreateCommand(
                    @"
DROP TABLE IF EXISTS foos;

CREATE TABLE foos(
  id SERIAL PRIMARY KEY NOT NULL,
  text VARCHAR(200));

INSERT INTO foos(text) VALUES 
  ('1'),
  ('2'),
  ('3'),
  ('4');
"))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        [Test]
        public void Dump()
        {
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
            {
                db.Open();
                using (var command = db.CreateCommand(@"SELECT * FROM foos
                                                        WHERE false"))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        foreach (var column in reader.GetColumnSchema())
                        {
                            Console.WriteLine($"Name: {column.ColumnName}, Type: {column.DataType}");
                        }
                    }
                }
            }
        }

        [Test]
        public void SchemaOnly()
        {
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
            {
                db.Open();
                using (var command = db.CreateCommand(@"SELECT * FROM foos"))
                {
                    using (var reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        foreach (var column in reader.GetColumnSchema())
                        {
                            Console.WriteLine($"Name: {column.ColumnName}, Type: {column.DataType}");
                        }
                    }
                }
            }
        }
    }
}
