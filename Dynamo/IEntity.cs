using System.Data;

namespace Dynamo
{
    public interface IEntity
    {
        void Populate(IDataReader reader);
    }
}