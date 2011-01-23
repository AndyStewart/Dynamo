using System.Collections.Generic;

namespace Dynamo
{
    public interface IRepository
    {
        IList<object> FindBySql(string sqlString);
        IList<object> FindBySql<T>(string sqlString) where T : Entity;
        void Save(Entity entity);
        T GetById<T>(int id) where T : Entity;
        void Delete<T>(T entity) where T : Entity;
    }
}