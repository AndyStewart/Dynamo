using System.Data;

namespace ActiveRecord
{
    internal interface IDbProvider
    {
        IDataReader ExecuteReader(string sql);
        object ExecuteScalar(string sql);
        void ExecuteNonQuery(string sql);
    }
}