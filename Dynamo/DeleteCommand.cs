using System.Data;
using Dynamo;

namespace Dynamo
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