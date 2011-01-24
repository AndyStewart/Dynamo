﻿using System;
using System.Data;

namespace Dynamo.Commands
{
    public class FindByIdCommand : ISqlCommand
    {
        private readonly Type entityType;
        private readonly int id;

        public FindByIdCommand(Type entityType, int id)
        {
            this.entityType = entityType;
            this.id = id;
        }

        public void Execute(IDbCommand dbCommand)
        {
            dbCommand.CommandText = "SELECT * FROM " + entityType.Name + " WHERE Id=" + id;

            using (var reader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (reader.Read())
                {
                    Result = (IEntity)Activator.CreateInstance(entityType);
                    Result.Populate(reader);
                }
            }
        }

        public IEntity Result { get; set; }
    }
}