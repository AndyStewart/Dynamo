using System;
using System.Collections.Generic;
using Dynamo.Provider;

namespace Dynamo
{
    public interface ISession
    {
        IList<object> FindBySql(string sqlString, object parameters = null);
        IList<object> FindBySql<T>(string sqlString, object parameters = null) where T : Entity;
        void Save(Entity entity);
        T GetById<T>(int id) where T : Entity;
        void Delete<T>(T entity) where T : Entity;
        object GetById(Type entityType, int id);
        IQuery Find<T>() where T : Entity;
        dynamic DynamicFind<T>() where T : Entity;
        IDbProvider DbProvider { get; }
        IEntityCache EntityCache { get; set; }
    }
}