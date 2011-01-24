using System.Data;

namespace Dynamo.Commands
{
    public interface ISqlCommand
    {
        void Execute(IDbCommand dbCommand);
    }
}