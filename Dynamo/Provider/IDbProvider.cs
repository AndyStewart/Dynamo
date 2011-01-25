using System.Data;
using Dynamo.Commands;

namespace Dynamo.Provider
{
    public interface IDbProvider
    {
        IDataReader ExecuteReader(string sql, object parameters);
        object ExecuteScalar(string sql);
        void ExecuteNonQuery(string sql);
        void ExecuteCommand(ISqlCommand sqlCommand);
    }
}