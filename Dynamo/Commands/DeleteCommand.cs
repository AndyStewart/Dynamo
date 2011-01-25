using System.Data;
using System.Data.SqlClient;

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
            dbCommand.CommandText = "DELETE FROM " + entity.GetType().Name + " WHERE Id=@Id";
            dbCommand.Parameters.Add(new SqlParameter("@Id", entity.Self.Id));
            dbCommand.ExecuteNonQuery();
        }
    }
}