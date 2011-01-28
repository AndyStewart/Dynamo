using System.Data;
using Dynamo.Provider;

namespace Dynamo
{
    public interface IEntity
    {
        ISession Session { get; set; }
        void Populate(IDataReader reader);
    }
}