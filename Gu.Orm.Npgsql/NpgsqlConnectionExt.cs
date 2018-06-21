namespace Gu.Orm.Npgsql
{
    using global::Npgsql;

    public static class NpgsqlConnectionExt
    {
        public static NpgsqlCommand CreateCommand(this NpgsqlConnection connection, string sql)
        {
            return new NpgsqlCommand(sql, connection);
        }

        public static NpgsqlCommand CreateCommand(this NpgsqlConnection connection, string sql, NpgsqlParameter parameter)
        {
            var command = new NpgsqlCommand(sql, connection);
            command.Parameters.Add(parameter);
            return command;
        }

        public static NpgsqlCommand CreateCommand(this NpgsqlConnection connection, string sql, NpgsqlParameter parameter1, NpgsqlParameter parameter2)
        {
            var command = new NpgsqlCommand(sql, connection);
            command.Parameters.Add(parameter1);
            command.Parameters.Add(parameter2);
            return command;
        }
    }
}