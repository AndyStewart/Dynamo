using System.Data;
using Dynamo.Provider;

namespace Dynamo
{
    public interface IEntity
    {
        IEntityCache EntityCache { get; set; }
        void Populate(IDataReader reader);
    }
}