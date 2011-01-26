using System;
using System.Collections.Generic;
using System.Linq;
using Dynamo.Commands;
using Dynamo.Provider;

namespace Dynamo
{
    public class Repository : IRepository
    {
        private readonly IDbProvider dbProvider;

        public Repository(string connectionString)
        {
            dbProvider = new SqlProvider(connectionString);
        }

        public IList<object> FindBySql(string sqlString, object parameters = null)
        {
            return FindBySql<Entity>(sqlString, parameters);
        }

        public IList<dynamic> FindBySql<T>(string sqlString, object parameters = null) where T : Entity
        {
            using (var reader = dbProvider.ExecuteReader(sqlString, parameters))
            {
                var results = new List<dynamic>();
                if (reader == null)
                    return results;

                while (reader.Read())
                {
                    var entity = (IEntity)Activator.CreateInstance<T>();
                    entity.Repository = this;
                    entity.Populate(reader);

                    results.Add(entity);
                }
                return results;
            }
        }

        public void Save(Entity entity)
        {
            if (entity.Repository == null)
                entity.Repository = this;

            if (entity.Properties.Any(q =>  q.PropertyName == "Id"))
            {
                dbProvider.ExecuteCommand(new UpdateCommand(entity));
                return;
            }

            dbProvider.ExecuteCommand(new InsertCommand(entity));
        }

        public T GetById<T>(int id) where T : Entity
        {
            return (T) GetById(typeof (T), id);
        }

        public object GetById(Type entityType, int id)
        {
            var command = new FindByIdCommand(entityType, id);
            dbProvider.ExecuteCommand(command);

            if (command.Result != null)
                command.Result.Repository = this;

            return command.Result;
        }


        public void Delete<T>(T entity) where T : Entity
        {
            dbProvider.ExecuteCommand(new DeleteCommand(entity));
        }

        public Query<T> Find<T>(string condition, object paramaters = null) where T : Entity
        {
            var query = new Query<T>(dbProvider);
            query.Where(condition, paramaters);
            return query;
        }

        public dynamic DynamicFind<T>() where T : Entity
        {
            return new Query<T>(dbProvider);
        }
    }
}
