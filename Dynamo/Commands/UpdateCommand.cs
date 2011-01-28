using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dynamo.Commands
{
    public class UpdateCommand : ISqlCommand   
    {
        private Entity entity;

        public UpdateCommand(Entity entity)
        {
            this.entity = entity;
        }

        public void Execute(IDbCommand dbCommand)
        {
            var entityType = entity.GetType();
            var sqlString = "UPDATE " + entityType.Name + " SET ";

            foreach (var property in entity.Properties.Where(q => q.PropertyName != "Id"))
            {
                sqlString += property.ColumnName + "= @" + property.PropertyName + ",";
                dbCommand.Parameters.Add(new SqlParameter(property.PropertyName, property.Value ?? DBNull.Value));
            }
                

            sqlString = sqlString.TrimEnd(',');
            sqlString += " WHERE Id=@Id";

            dbCommand.Parameters.Add(new SqlParameter("@Id", entity.Self.Id));

            dbCommand.CommandText = sqlString;
            dbCommand.ExecuteNonQuery();
        }

        private static string ValueEncode(object value)
        {
            if (value == null)
                return "null";

            var valueType = value.GetType();

            if (valueType == typeof(string))
                return "'" + value + "'";

            return value.ToString();
        }
    }
}