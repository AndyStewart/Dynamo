using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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
            foreach (var property in entity.Properties.Where(q => q.PropertyType != PropertyType.HasMany))
            {
                columnNameString += property.ColumnName + ",";
                valueString += "@" + property.PropertyName + ",";

                var value = property.Value;
                var propertyEntity = property.Value as Entity;
                if (propertyEntity != null)
                    value = propertyEntity.Self.Id;

                dbCommand.Parameters.Add(new SqlParameter("@" + property.PropertyName, value ?? DBNull.Value));
            }

            columnNameString = columnNameString.TrimEnd(',');
            valueString = valueString.TrimEnd(',');

            sqlString = sqlString + "(" + columnNameString + ") VALUES (" + valueString + "); SELECT @@IDENTITY";

            dbCommand.CommandText = sqlString;

            ((dynamic) entity).Id = Convert.ToInt32(dbCommand.ExecuteScalar());
        }
    }
}