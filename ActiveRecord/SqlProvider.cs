using System.Data;
using System.Data.SqlClient;

namespace ActiveRecord
{
    internal class SqlProvider : IDbProvider
    {
        private readonly string connectionString;

        public SqlProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDataReader ExecuteReader(string sql)
        {
            return CreateCommand(sql).ExecuteReader(CommandBehavior.CloseConnection);
        }

        private SqlCommand CreateCommand(string sql)
        {
            SqlConnection sqlConnection = GetSqlConnection();
            return new SqlCommand(sql, sqlConnection);
        }

        private SqlConnection GetSqlConnection()
        {
            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            return sqlConnection;
        }

        public object ExecuteScalar(string sql)
        {
            return CreateCommand(sql).ExecuteScalar();
        }
    }
}