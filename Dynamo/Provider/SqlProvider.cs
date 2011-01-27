using System;
using System.Data;
using System.Data.SqlClient;
using Dynamo.Commands;

namespace Dynamo.Provider
{
    public class SqlProvider : IDbProvider
    {
        private readonly string connectionString;
        private SqlConnection sqlConnection;

        public SqlProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDataReader ExecuteReader(string sql, object parameters)
        {
            var command = CreateCommand(sql);

            if (parameters != null)
            {
                foreach (var parameter in parameters.GetType().GetProperties())
                    command.Parameters.Add(new SqlParameter("@" + parameter.Name, parameter.GetValue(parameters, null)));
            }
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        private SqlCommand CreateCommand(string sql)
        {
            return new SqlCommand(sql, GetSqlConnection());
        }

        private SqlConnection GetSqlConnection()
        {
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            return sqlConnection;
        }

        public object ExecuteScalar(string sql)
        {
            var value = CreateCommand(sql).ExecuteScalar();
            sqlConnection.Close();
            return value;
        }

        public void ExecuteNonQuery(string sql)
        {
            CreateCommand(sql).ExecuteNonQuery();
            sqlConnection.Close();
        }

        public void ExecuteCommand(ISqlCommand sqlCommand)
        {
            sqlCommand.Execute(CreateCommand(null));
            sqlConnection.Close();
        }

        public int QueryCount { get; private set;}
    }
}   