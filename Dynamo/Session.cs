using System;
using System.Collections.Generic;
using System.Linq;
using Dynamo.Commands;
using Dynamo.Provider;

namespace Dynamo
{
    public class Session : ISession
    {
        public Session(string connectionString)
        {
            DbProvider = new SqlProvider(connectionString);
            EntityCache = new EntityCache(this);
        }

        public IEntityCache EntityCache { get; set; }

        public IDbProvider DbProvider { get; private set; }
        public IList<object> FindBySql(string sqlString, object parameters = null)
        {
            return FindBySql<Entity>(sqlString, parameters);
        }

        public IList<dynamic> FindBySql<T>(string sqlString, object parameters = null) where T : Entity
        {
            using (var reader = DbProvider.ExecuteReader(sqlString, parameters))
            {
                var results = new List<dynamic>();
                if (reader == null)
                    return results;

                while (reader.Read())
                {
                    var entity = (IEntity)Activator.CreateInstance<T>();
                    entity.EntityCache = EntityCache;
                    entity.Populate(reader);
                    results.Add(entity);
                }
                return results;
            }
        }

        public void Save(Entity entity)
        {
            if (entity.EntityCache == null)
                entity.EntityCache = EntityCache;

            if (entity.Properties.Any(q =>  q.PropertyName == "Id" && q.Value != null))
            {
                DbProvider.ExecuteCommand(new UpdateCommand(entity));
                EntityCache.Add(entity);
                return;
            }

            DbProvider.ExecuteCommand(new InsertCommand(entity));
            EntityCache.Add(entity);
        }

        public T GetById<T>(int id) where T : Entity
        {
            return (T) GetById(typeof (T), id);
        }

        public object GetById(Type entityType, int id)
        {
            var command = new FindByIdCommand(entityType, id, EntityCache);
            DbProvider.ExecuteCommand(command);

            if (command.Result != null)
                command.Result.EntityCache = EntityCache;

            return command.Result;
        }


        public void Delete<T>(T entity) where T : Entity
        {
            DbProvider.ExecuteCommand(new DeleteCommand(entity));
        }

        public Query<T> Find<T>() where T : Entity
        {
            return new Query<T>(DbProvider, this);
        }

        public dynamic DynamicFind<T>() where T : Entity
        {
            return new Query<T>(DbProvider, this);
        }
    }
}
