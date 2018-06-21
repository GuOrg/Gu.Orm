namespace Gu.Orm.Npgsql
{
    using System;
    using System.Collections.Generic;
    using global::Npgsql;

    public static class Database
    {
        [Obsolete("This is for triggering the analyzer to create the func.", error: true)]
        public static bool QuerySingle<T>(string connectionString, string sql, NpgsqlParameter parameter, out T result)
        {
            throw new NotSupportedException();
        }

        public static bool QuerySingle<T>(string connectionString, string sql, NpgsqlParameter parameter, Func<NpgsqlDataReader, T> read, out T result)
        {
            using (var db = new NpgsqlConnection(connectionString))
            {
                db.Open();
                using (var command = db.CreateCommand(sql, parameter))
                {
                    return command.MapSingle(read, out result);
                }
            }
        }

        [Obsolete("This is for triggering the analyzer to create the func.", error: true)]
        public static IEnumerable<T> QuerySingle<T>(string connectionString, string sql)
        {
            throw new NotSupportedException();
        }

        public static IEnumerable<T> QuerySingle<T>(string connectionString, string sql, Func<NpgsqlDataReader, T> read)
        {
            using (var db = new NpgsqlConnection(connectionString))
            {
                db.Open();
                using (var command = db.CreateCommand(sql))
                {
                    return command.Map(read);
                }
            }
        }
    }
}
