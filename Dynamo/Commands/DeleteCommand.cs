﻿using System.Data;

namespace Dynamo.Commands
{
    public class DeleteCommand : ISqlCommand
    {
        private readonly Entity entity;

        public DeleteCommand(Entity entity)
        {
            this.entity = entity;
        }

        public void Execute(IDbCommand dbCommand)
        {
            dbCommand.CommandText = "DELETE FROM " + entity.GetType().Name + " WHERE Id=" + entity.Self.Id;
            dbCommand.ExecuteNonQuery();
        }
    }
}