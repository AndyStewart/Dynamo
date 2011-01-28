using System;
using System.Collections.Generic;
using System.Linq;
using Dynamo.Commands;
using Dynamo.Provider;

namespace Dynamo
{
    public class Session : ISession
    {
        public IList<CachedItem> Cache { get; set; }

        public Session(string connectionString)
        {
            DbProvider = new SqlProvider(connectionString);
            Cache = new List<CachedItem>();
        }

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
                    entity.Session = this;
                    entity.Populate(reader);
                    results.Add(entity);
                }
                return results;
            }
        }

        public void Save(Entity entity)
        {
            if (entity.Session == null)
                entity.Session = this;

            if (entity.Properties.Any(q =>  q.PropertyName == "Id" && q.Value != null))
            {
                DbProvider.ExecuteCommand(new UpdateCommand(entity));
                return;
            }

            DbProvider.ExecuteCommand(new InsertCommand(entity));
        }

        public T GetById<T>(int id) where T : Entity
        {
            return (T) GetById(typeof (T), id);
        }

        public object GetById(Type entityType, int id)
        {
            var command = new FindByIdCommand(entityType, id, this);
            DbProvider.ExecuteCommand(command);

            if (command.Result != null)
                command.Result.Session = this;

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

    public class CachedItem
    {
        public int Id { get; set; }
        public Type Type { get; set; }
        public Entity Value { get; set; }
    }
}
