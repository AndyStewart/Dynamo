using System;
using System.Collections;
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
            Result = new ArrayList();

            var sqlString = "SELECT ";

            if (query.Mode == QueryMode.Count)
            {
                sqlString += "Count(*) ";
            }
            else
            {
                sqlString += typeof (T).Name + ".* ";
            }

            sqlString += "FROM " + typeof (T).Name;

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
                    switch (query.Mode)
                    {
                        case QueryMode.Count:
                            Result.Add(reader[0]);
                            return;
                        case QueryMode.Queries:
                            var entity = Activator.CreateInstance<T>();
                            entity.EntityCache = query.Session.EntityCache;
                            entity.Populate(reader);
                            Result.Add(entity);
                            break;
                            

                    }

                }
            }
        }

        public ArrayList Result { get; set; }
    }
}