using System;
using System.Collections.Generic;

namespace Dynamo
{
    public interface IRepository
    {
        IList<object> FindBySql(string sqlString, object parameters = null);
        IList<object> FindBySql<T>(string sqlString, object parameters = null) where T : Entity;
        void Save(Entity entity);
        T GetById<T>(int id) where T : Entity;
        void Delete<T>(T entity) where T : Entity;
        object GetById(Type entityType, int id);
        Query<T> Find<T>(string condition, object paramaters = null) where T : Entity;
    }
}