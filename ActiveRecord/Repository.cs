using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;

namespace ActiveRecord
{

    public class Repository
    {
        private string ConnectionString = @"Data Source=.\sql2005;Initial Catalog=BobsDb;Integrated Security=True";

        public IList<dynamic> FindBySql(string sqlString)
        {
            return FindBySql<Entity>(sqlString);
        }

        public IList<dynamic> FindBySql<T>(string sqlString)
        {
            var sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            var command = new SqlCommand(sqlString, sqlConnection);

            using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
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
                reader.Close();
                return results;
            }
        }

        public void Save(Entity entity)
        {
            var sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

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

            var command = new SqlCommand(sqlString, sqlConnection);
            var result = command.ExecuteScalar();
            ((dynamic) entity).Id = result;
        }

        private string valueEncode(object value)
        {
            var valueType = value.GetType();

            if (valueType == typeof(string))
                return "'" + value + "'";

            return value.ToString();
        }
    }

    public interface IEntity
    {
        void Populate(IDataReader reader);
    }

    public class Entity : DynamicObject, IEntity
    {
        protected dynamic self;

        public Entity()
        {
            Properties = new Dictionary<string, object>();
            self = this;
        }

        public Dictionary<string, object> Properties { get; set; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Properties[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Properties[binder.Name] = value;
            return true;
        }

        public void Populate(IDataReader reader)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                Properties.Add(reader.GetName(i), reader[i]);
            }
        }
    }
}
