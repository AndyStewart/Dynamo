using System;
using System.Collections.Generic;
using System.Linq;

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

        public IList<dynamic> FindBySql<T>(string sqlString) where T : Entity
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
            if (entity.Properties.ContainsKey("Id"))
            {
                ExecuteUpdateCommand(entity);
            }
            else
            {
                ExecuteInsert(entity);
            }
        }

        private void ExecuteUpdateCommand(Entity entity)
        {
            var entityType = entity.GetType();
            var sqlString = "UPDATE " + entityType.Name + " SET ";

            foreach (var property in entity.Properties.Where(q => q.Key != "Id"))
                sqlString += property.Key + "=" + ValueEncode(property.Value) + ",";

            sqlString = sqlString.TrimEnd(',');
            sqlString += " WHERE Id=" + entity.Properties["Id"];
            dbProvider.ExecuteNonQuery(sqlString);
        }

        private void ExecuteInsert(Entity entity)
        {
            var entityType = entity.GetType();
            var sqlString = "INSERT INTO " + entityType.Name;

            var columnNameString = "";
            var valueString = "";
            foreach (var property in entity.Properties)
            {
                columnNameString += property.Key + ",";
                valueString += ValueEncode(property.Value) + ",";
            }

            columnNameString = columnNameString.TrimEnd(',');
            valueString = valueString.TrimEnd(',');

            sqlString = sqlString + "(" + columnNameString + ") VALUES (" + valueString + "); SELECT @@IDENTITY";

            ((dynamic) entity).Id = Convert.ToInt32(dbProvider.ExecuteScalar(sqlString));
        }

        private static string ValueEncode(object value)
        {
            var valueType = value.GetType();

            if (valueType == typeof(string))
                return "'" + value + "'";

            return value.ToString();
        }

        public T GetById<T>(int id) where T : Entity
        {
            var results = FindBySql<T>("SELECT * FROM " + typeof (T).Name + " WHERE Id=" + id);
            return results.Count == 0 ? null : results[0];
        }

        public void Delete<T>(T contact) where T : Entity
        {
            dbProvider.ExecuteNonQuery("DELETE FROM " + typeof (T).Name + " WHERE Id=" + ((dynamic) contact).Id);
        }
    }
}
