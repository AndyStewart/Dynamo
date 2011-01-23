using System;
using System.Data;
using System.Linq;

namespace Dynamo
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

            foreach (var property in entity.Properties.Where(q => q.Key != "Id"))
                sqlString += property.Key + "=" + ValueEncode(property.Value) + ",";

            sqlString = sqlString.TrimEnd(',');
            sqlString += " WHERE Id=" + entity.Properties["Id"];

            dbCommand.CommandText = sqlString;
            dbCommand.ExecuteNonQuery();
        }

        private static string ValueEncode(object value)
        {
            var valueType = value.GetType();

            if (valueType == typeof(string))
                return "'" + value + "'";

            return value.ToString();
        }
    }
}