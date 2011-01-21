using System.Data;

namespace ActiveRecord
{
    public interface IEntity
    {
        void Populate(IDataReader reader);
    }
}