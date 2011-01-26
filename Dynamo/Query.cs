using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dynamo.Commands;
using Dynamo.Provider;

namespace Dynamo
{
    public class Query<T> where T : Entity
    {
        private readonly IDbProvider dbProvider;

        public Query(IDbProvider dbProvider)
        {
            this.dbProvider = dbProvider;
        }

        public string OrderClause { get; set; }
        public object ConditionParamaters { get; set; }
        public string Condition { get; set; }

        public void Where(string condition, object paramaters)
        {
            Condition = condition;
            ConditionParamaters = paramaters;
        }

        public IList<dynamic> ToList()
        {
            return ExecuteQuery().Cast<dynamic>().ToList();
        }

        private IEnumerable<T> ExecuteQuery()
        {
            var command = new FindCommand<T>(this);
            dbProvider.ExecuteCommand(command);
            return command.Result;
        }

        public IEnumerator GetEnumerator()
        {
            return ExecuteQuery().GetEnumerator();
        }

        public Query<T> OrderBy(string orderClause)
        {
            OrderClause = orderClause;
            return this;
        }
    }
}