using System;
using System.Data;
using System.Linq;

namespace Dynamo.Commands
{
    public class InsertCommand : ISqlCommand
    {
        private readonly Entity entity;

        public InsertCommand(Entity entity)
        {
            this.entity = entity;
        }

        public void Execute(IDbCommand dbCommand)
        {
            var entityType = entity.GetType();
            var sqlString = "INSERT INTO " + entityType.Name;

            var columnNameString = "";
            var valueString = "";
            foreach (var property in entity.Properties.Where(q => q.Type != PropertyType.HasMany))
            {
                columnNameString += property.ColumnName + ",";
                valueString += ValueEncode(property.Value) + ",";
            }

            columnNameString = columnNameString.TrimEnd(',');
            valueString = valueString.TrimEnd(',');

            sqlString = sqlString + "(" + columnNameString + ") VALUES (" + valueString + "); SELECT @@IDENTITY";

            dbCommand.CommandText = sqlString;

            ((dynamic) entity).Id = Convert.ToInt32(dbCommand.ExecuteScalar());
        }

        private static string ValueEncode(object value)
        {
            var entity = value as Entity;
            if (entity != null)
                return entity.Self.Id.ToString();

            if (value == null)
                return "null";

            var valueType = value.GetType();

            if (valueType == typeof(string))
                return "'" + value + "'";

            return value.ToString();
        }
    }
}