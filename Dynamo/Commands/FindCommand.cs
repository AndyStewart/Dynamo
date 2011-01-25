using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Dynamo.Commands
{
    public class FindCommand<T> : ISqlCommand where T : Entity
    {
        private readonly string condition;
        private readonly object paramaters;

        public FindCommand(string condition, object paramaters)
        {
            this.condition = condition;
            this.paramaters = paramaters;
        }

        public void Execute(IDbCommand dbCommand)
        {
            Result = new List<T>();

            var sqlString = "SELECT * FROM " + typeof (T).Name + " Where " + condition;
            dbCommand.CommandText = sqlString;

            if (paramaters != null)
            {
                foreach (var property in paramaters.GetType().GetProperties())
                    dbCommand.Parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(paramaters, null)));
            }

            using (var reader = dbCommand.ExecuteReader())
            {
                while(reader.Read())
                {
                    T entity = Activator.CreateInstance<T>();
                    entity.Populate(reader);
                    Result.Add(entity);
                }
            }
        }

        public IList<T> Result { get; set; }
    }
}