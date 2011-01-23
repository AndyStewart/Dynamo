using System.Data;

namespace Dynamo
{
    public interface ISqlCommand
    {
        void Execute(IDbCommand dbCommand);
    }
}