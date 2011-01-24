using System;
using System.Collections.Generic;
using System.Linq;
using Dynamo;
using Dynamo.Commands;
using Dynamo.Provider;

namespace Dynamo
{
    public class Repository : IRepository
    {
        private const string ConnectionString = @"Data Source=.\sqlexpress;Initial Catalog=Dynamo_Test;Integrated Security=True";

        private IDbProvider dbProvider = new SqlProvider(ConnectionString);

        public IList<dynamic> FindBySql(string sqlString)
        {
            return FindBySql<Entity>(sqlString);
        }

        public IList<dynamic> FindBySql<T>(string sqlString) where T : Entity
        {
            using (var reader = dbProvider.ExecuteReader(sqlString))
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
            command.Result.Repository = this;
            return command.Result;
        }


        public void Delete<T>(T entity) where T : Entity
        {
            dbProvider.ExecuteCommand(new DeleteCommand(entity));
        }

    }
}
