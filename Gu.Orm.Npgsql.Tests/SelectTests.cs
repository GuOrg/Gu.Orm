namespace Gu.Orm.Npgsql.Tests
{
    using System.Configuration;
    using System.Linq;
    using global::Npgsql;
    using NUnit.Framework;

    public class SelectTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
            {
                db.Open();
                var command = db.CreateCommand(@"
DROP TABLE IF EXISTS foos;

CREATE TABLE foos(
  id SERIAL PRIMARY KEY NOT NULL,
  text VARCHAR(200));

INSERT INTO foos(text) VALUES 
  ('1'),
  ('2'),
  ('3'),
  ('4');
");
                command.ExecuteNonQuery();
            }
        }

        [Test]
        public void Map()
        {
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
            {
                db.Open();
                using (var command = db.CreateCommand("SELECT id, text FROM foos"))
                {
                    var foos = command.Map(x => new ImmutableFoo(x.GetInt32(0), x.GetString(1))).ToList();
                    CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, foos.Select(x => x.Id));
                    CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, foos.Select(x => x.Text));
                }
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void MapSingleWhenMatch(int id)
        {
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
            {
                db.Open();
                using (var command = db.CreateCommand(
                    "SELECT id, text FROM foos WHERE id = @id",
                    new NpgsqlParameter("id", id)))
                {
                    Assert.AreEqual(true, command.MapSingle(x => new ImmutableFoo(x.GetInt32(0), x.GetString(1)), out var foo));
                    Assert.AreEqual(id, foo.Id);
                    Assert.AreEqual(id.ToString(), foo.Text);
                }
            }
        }

        [Test]
        public void MapSingleNoMatch()
        {
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
            {
                db.Open();
                using (var command = db.CreateCommand(
                    "SELECT id, text FROM foos WHERE id = @id",
                    new NpgsqlParameter("id", 5)))
                {
                    Assert.AreEqual(false, command.MapSingle(x => new ImmutableFoo(x.GetInt32(0), x.GetString(1)), out _));
                }
            }
        }

        public class ImmutableFoo
        {
            public ImmutableFoo(int id, string text)
            {
                this.Id = id;
                this.Text = text;
            }

            public int Id { get; }

            public string Text { get; }
        }
    }
}
