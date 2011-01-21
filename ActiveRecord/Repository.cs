using System;
using System.Collections.Generic;

namespace ActiveRecord
{
    public class Repository
    {
        private const string ConnectionString = @"Data Source=.\sql2005;Initial Catalog=BobsDb;Integrated Security=True";

        private IDbProvider dbProvider = new SqlProvider(ConnectionString);

        public IList<dynamic> FindBySql(string sqlString)
        {
            return FindBySql<Entity>(sqlString);
        }

        public IList<dynamic> FindBySql<T>(string sqlString)
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
            var entityType = entity.GetType();
            var sqlString = "INSERT INTO " + entityType.Name;

            var columnNameString = "";
            var valueString = "";
            foreach (var property in entity.Properties)
            {
                columnNameString += property.Key + ",";
                valueString += valueEncode(property.Value) + ",";
            }

            columnNameString = columnNameString.TrimEnd(',');
            valueString = valueString.TrimEnd(',');

            sqlString = sqlString + "(" + columnNameString + ") VALUES (" + valueString + "); SELECT @@IDENTITY";

            ((dynamic) entity).Id = dbProvider.ExecuteScalar(sqlString);
        }

        private string valueEncode(object value)
        {
            var valueType = value.GetType();

            if (valueType == typeof(string))
                return "'" + value + "'";

            return value.ToString();
        }
    }
}
