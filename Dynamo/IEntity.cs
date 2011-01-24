using System.Data;
using Dynamo.Provider;

namespace Dynamo
{
    public interface IEntity
    {
        IRepository Repository { get; set; }
        void Populate(IDataReader reader);
    }
}