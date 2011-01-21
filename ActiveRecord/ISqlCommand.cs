using System.Data;

namespace ActiveRecord
{
    public interface ISqlCommand
    {
        void Execute(IDbCommand dbCommand);
    }
}