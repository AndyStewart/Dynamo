using System.Data;

namespace Dynamo
{
    internal interface IDbProvider
    {
        IDataReader ExecuteReader(string sql);
        object ExecuteScalar(string sql);
        void ExecuteNonQuery(string sql);
        void ExecuteCommand(ISqlCommand sqlCommand);
    }
}