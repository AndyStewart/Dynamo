using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Dynamo.Commands
{
    public class FindCommand<T> : ISqlCommand where T : Entity
    {
        private readonly Query<T> query;

        public FindCommand(Query<T> query)
        {
            this.query = query;
        }

        public void Execute(IDbCommand dbCommand)
        {
            Result = new List<T>();

            var sqlString = "SELECT * FROM " + typeof (T).Name + " Where " + query.Condition;
            dbCommand.CommandText = sqlString;

            if (query.ConditionParamaters!= null)
            {
                foreach (var property in query.ConditionParamaters.GetType().GetProperties())
                    dbCommand.Parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(query.ConditionParamaters, null)));
            }

            if (!String.IsNullOrEmpty(query.OrderClause))
                dbCommand.CommandText += " ORDER BY " + query.OrderClause;


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