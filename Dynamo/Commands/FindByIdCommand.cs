using System;
using System.Data;
using System.Data.SqlClient;

namespace Dynamo.Commands
{
    public class FindByIdCommand : ISqlCommand
    {
        private readonly Type entityType;
        private readonly int id;
        private readonly EntityCache entityCache;

        public FindByIdCommand(Type entityType, int id, IEntityCache entityCache)
        {
            this.entityType = entityType;
            this.id = id;
            this.entityCache = (EntityCache) entityCache;
        }

        public void Execute(IDbCommand dbCommand)
        {
            var entity = (Entity)Activator.CreateInstance(entityType);
            dbCommand.CommandText = "SELECT * FROM " + entity.TableName + " WHERE Id=@Id";
            dbCommand.Parameters.Add(new SqlParameter("Id", id));

            using (var reader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (reader.Read())
                {
                    Result = (IEntity)Activator.CreateInstance(entityType);
                    Result.EntityCache = entityCache;
                    Result.Populate(reader);
                }
            }
        }

        public IEntity Result { get; set; }
    }
}