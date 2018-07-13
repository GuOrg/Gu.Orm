namespace Gu.Orm.Npgsql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using global::Npgsql;

    public static class NpgsqlCommandExt
    {
        [Obsolete("This is for triggering the analyzer to create the func.", error: true)]
        public static bool MapSingle<T>(this NpgsqlCommand command, out T result)
        {
            throw new NotSupportedException();
        }

        public static bool MapSingle<T>(this NpgsqlCommand command, Func<NpgsqlDataReader, T> read, out T result)
        {
            using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (reader.Read())
                {
                    result = read(reader);
                    return true;
                }
            }

            result = default;
            return false;
        }

        [Obsolete("This is for triggering the analyzer to create the func.", error: true)]
        public static IEnumerable<T> Map<T>(this NpgsqlCommand command)
        {
            throw new NotSupportedException();
        }

        public static IEnumerable<T> Map<T>(this NpgsqlCommand command, Func<NpgsqlDataReader, T> read)
        {
            using (var reader = command.ExecuteReader(CommandBehavior.Default))
            {
                while (reader.Read())
                {
                    yield return read(reader);
                }
            }
        }
    }
}
