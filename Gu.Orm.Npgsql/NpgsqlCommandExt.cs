namespace Gu.Orm.Npgsql
{
    using System;
    using System.Data;
    using global::Npgsql;

    public static class NpgsqlCommandExt
    {
        [Obsolete("This is for triggering the analyzer to create the func.", error: true)]
        public static bool TrySingle<T>(this NpgsqlCommand command, out T result)
        {
            throw new NotImplementedException();
        }

        public static bool TrySingle<T>(this NpgsqlCommand command, Func<NpgsqlDataReader, T> read, out T result)
        {
            using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (reader.Read())
                {
                    result = read(reader);
                    return true;
                }
            }

            result = default(T);
            return false;
        }
    }
}
