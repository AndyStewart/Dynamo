using System;
using System.Collections.Generic;
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
                    entity.Populate(reader);
                    results.Add(entity);
                }
                return results;
            }
        }

        public void Save(Entity entity)
        {
            if (entity.Properties.ContainsKey("Id"))
            {
                dbProvider.ExecuteCommand(new UpdateCommand(entity));
                return;
            }

            dbProvider.ExecuteCommand(new InsertCommand(entity));
        }

        public T GetById<T>(int id) where T : Entity
        {
            var command = new FindByIdCommand<T>(id);
            dbProvider.ExecuteCommand(command);
            return command.Result;
        }

        public void Delete<T>(T entity) where T : Entity
        {
            dbProvider.ExecuteCommand(new DeleteCommand(entity));
        }
    }
}
