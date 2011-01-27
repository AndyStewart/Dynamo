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

            var sqlString = "SELECT * FROM " + typeof (T).Name;

            if (!String.IsNullOrEmpty(query.Condition))
                sqlString += " Where " + query.Condition;
            
            dbCommand.CommandText = sqlString;

            if (query.ConditionParamaters!= null)
            {
                foreach (var property in query.ConditionParamaters)
                    dbCommand.Parameters.Add(new SqlParameter(property.Key, property.Value));
            }

            if (!String.IsNullOrEmpty(query.OrderClause))
                dbCommand.CommandText += " ORDER BY " + query.OrderClause;


            using (var reader = dbCommand.ExecuteReader())
            {
                while(reader.Read())
                {
                    var entity = Activator.CreateInstance<T>();
                    entity.Populate(reader);
                    Result.Add(entity);
                }
            }
        }

        public IList<T> Result { get; set; }
    }
}